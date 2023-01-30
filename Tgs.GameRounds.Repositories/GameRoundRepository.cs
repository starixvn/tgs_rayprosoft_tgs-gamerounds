using System.Runtime.CompilerServices;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories;

internal class GameRoundRepository : IGameRound
{
    private readonly BetContext _context;
    private readonly ILogger<GameRoundRepository> _logger;
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    public GameRoundRepository(
        BetContext context,
        ILogger<GameRoundRepository> logger,
        ISerialNumberGenerator serialNumberGenerator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
    }

    public async ValueTask<Guid?> AddCompanyRoundAsync(
        AddCompanyRound source,
        CancellationToken cancellationToken = default)
    {
        var roundId = await _serialNumberGenerator.GenerateAsync(
                            cancellationToken: cancellationToken).ConfigureAwait(false);

        var systemDateTimeOffset = DateTime.UtcNow;

        var data = new Round()
        {
            Id = roundId,
            ExternalRoundId = source.ExternalRoundId,
            GameProviderId = source.GameProviderId,
            PlayerCode = source.PlayerCode,
            GameId = source.GameId,
            CurrencyCode = source.CurrencyCode,
            Status = (short)GameRoundStatus.Closed,
            GameProviderConfigurationId = source.GameProviderConfigurationId,
            WalletId = source.WalletId,
            CreatedOn = systemDateTimeOffset,
            ClosedOn = systemDateTimeOffset,
            UpdatedOn = systemDateTimeOffset,
            BrandId = source.BrandId,
            ExternalCreatedOn = DateTime.SpecifyKind(source.ExternalCreatedOn.UtcDateTime, DateTimeKind.Utc),
            ExternalClosedOn = DateTime.SpecifyKind(source.ExternalClosedOn.UtcDateTime, DateTimeKind.Utc),
            BetAmount = source.BetAmount,
            ValidBetAmount = source.ValidBetAmount,
            SettleAmount = source.SettleAmount,
            Commission = source.Commission,
            Turnover = source.Turnover,
            WinLoss = source.CompanyWinLoss,
            ClientStatus = (short)source.ClientStatus
        };
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _ = await _context.Rounds
                        .AddAsync(
                            entity: data,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var getRound = await _context.Rounds.FirstAsync(
                x => x.Id == roundId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var roundDetail = new RoundDetail()
            {
                RoundId = getRound.Id,
                AdditionalData = string.Empty,
            };

            _ = await _context.RoundDetails
                        .AddAsync(
                            entity: roundDetail,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var recordId = await _serialNumberGenerator
                                    .GenerateAsync(
                                        cancellationToken: cancellationToken)
                                    .ConfigureAwait(false);

            var record = new RoundOperationRecord()
            {
                Id = recordId,
                RoundId = roundId,
                OperatorId = source.OperatorId,
                OldData = JsonSerializer.Serialize(new { }),
                NewData = JsonSerializer.Serialize(data),
                CreatedOn = systemDateTimeOffset,
                Remark = source.Remark
            };

            _ = await _context
                        .RoundOperationRecords
                        .AddAsync(
                            entity: record,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            _ = await _context
                        .SaveChangesAsync(
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            await transaction
                    .CommitAsync(
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            return roundId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{DateTime.Now.ToString(\"o\")} Data: {data} Message:添加新局出現例外", DateTime.Now.ToString("o"), data);
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            return null;
        }
    }

    public async ValueTask<Guid?> AddRoundAsync(
        AddRound source,
        CancellationToken cancellationToken = default)
    {
        var roundId = await _serialNumberGenerator.GenerateAsync(
                            cancellationToken: cancellationToken).ConfigureAwait(false);
        var round = new Round()
        {
            Id = roundId,
            ExternalRoundId = source.ExternalRoundId,
            GameProviderId = source.GameProviderId,
            PlayerCode = source.PlayerCode,
            GameId = source.GameId,
            CurrencyCode = source.CurrencyCode,
            Status = (short)source.Status,
            GameProviderConfigurationId = source.GameProviderConfigurationId,
            WalletId = source.WalletId,
            CreatedOn = DateTime.UtcNow,
            ClosedOn = (source.Status is GameRoundStatus.Closed) ? DateTime.UtcNow : null,
            UpdatedOn = DateTime.UtcNow,
            BrandId = source.BrandId,
            ExternalCreatedOn = DateTime.SpecifyKind(source.ExternalCreatedOn.UtcDateTime, DateTimeKind.Utc),
            ExternalClosedOn = source.ExternalClosedOn.HasValue ? DateTime.SpecifyKind(source.ExternalClosedOn.Value.UtcDateTime, DateTimeKind.Utc) : null,
            BetAmount = source.BetAmount,
            ValidBetAmount = source.ValidBetAmount,
            SettleAmount = source.SettleAmount,
            Commission = source.Commission,
            Turnover = source.Turnover,
            WinLoss = source.PlayerWinLoss.HasValue ? -source.PlayerWinLoss : null,
            ClientStatus = (short)source.ClientStatus
        };
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _ = await _context.Rounds
                        .AddAsync(
                            entity: round,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var getRound = await _context.Rounds.FirstAsync(
                x => x.Id == roundId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var roundDetail = new RoundDetail()
            {
                RoundId = getRound.Id,
                AdditionalData = source.AdditionalData ?? string.Empty,
            };

            _ = await _context.RoundDetails
                        .AddAsync(
                            entity: roundDetail,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return roundId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{DateTime.Now.ToString(\"o\")} Data: {round} Message:添加新局出現例外", DateTime.Now.ToString("o"), round);
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            return null;
        }
    }

    public async IAsyncEnumerable<Guid> AddRoundBatchAsync(
        IEnumerable<AddRoundBatch> sources,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var addDatas = from source in sources
                       join round in _context.Rounds.AsQueryable()
                       on new { source.ExternalRoundId, source.GameProviderId, source.GameId, source.WalletId }
                       equals new { round.ExternalRoundId, round.GameProviderId, round.GameId, round.WalletId }
                       into addRounds
                       from addRound in addRounds.DefaultIfEmpty()
                       where addRound is null
                       select source;

        if (!addDatas.Any())
        {
            yield break;
        }

        var rounds = new List<Round>();
        var roundDetails = new List<RoundDetail>();

        foreach (var addData in addDatas)
        {
            var roundId = await _serialNumberGenerator
                                .GenerateAsync(
                                    cancellationToken: cancellationToken)
                                .ConfigureAwait(false);

            rounds.Add(new Round()
            {
                Id = roundId,
                ExternalRoundId = addData.ExternalRoundId,
                GameProviderId = addData.GameProviderId,
                PlayerCode = addData.PlayerCode,
                GameId = addData.GameId,
                CurrencyCode = addData.CurrencyCode,
                Status = (short)addData.Status,
                GameProviderConfigurationId = addData.GameProviderConfigurationId,
                WalletId = addData.WalletId,
                CreatedOn = DateTime.UtcNow,
                ClosedOn = (addData.Status is GameRoundStatus.Closed) ? DateTime.UtcNow : null,
                UpdatedOn = DateTime.UtcNow,
                BrandId = addData.BrandId,
                ExternalCreatedOn = DateTime.SpecifyKind(addData.ExternalCreatedOn.UtcDateTime, DateTimeKind.Utc),
                ExternalClosedOn = addData.ExternalClosedOn.HasValue ? DateTime.SpecifyKind(addData.ExternalClosedOn.Value.UtcDateTime, DateTimeKind.Utc) : null,
                BetAmount = addData.BetAmount,
                ValidBetAmount = addData.ValidBetAmount,
                SettleAmount = addData.SettleAmount,
                Commission = addData.Commission,
                Turnover = addData.Turnover,
                WinLoss = addData.PlayerWinLoss.HasValue ? -addData.PlayerWinLoss : null,
                ClientStatus = (short)addData.ClientStatus
            });

            roundDetails.Add(new RoundDetail()
            {
                RoundId = roundId,
                AdditionalData = addData.AdditionalData ?? string.Empty,
            });
        }

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            await _context.Rounds
                .AddRangeAsync(
                    entities: rounds,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            _ = await _context
                .SaveChangesAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await _context.RoundDetails
                .AddRangeAsync(
                    entities: roundDetails,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            _ = await _context
                .SaveChangesAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await transaction
                    .CommitAsync(
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{DateTime.Now.ToString(\"o\")} Data: {round} Message:批此新增新局出現例外", DateTime.Now.ToString("o"));
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);

            yield break;
        }

        foreach (var round in rounds)
        {
            yield return round.Id;
        }
    }

    public async ValueTask AdjustClosedRoundAsync(
        Guid roundId,
        AdjustClosedRound source,
        CancellationToken cancellationToken = default)
    {
        var data = await _context.Rounds
            .Where(x => x.Id == roundId)
            .FirstAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var oldData = data;

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var systemDateTimeOffset = DateTime.UtcNow;
            data.UpdatedOn = systemDateTimeOffset;
            data.ClosedOn = systemDateTimeOffset;
            data.ExternalClosedOn = source.ExternalClosedOn.UtcDateTime;
            data.BetAmount = source.BetAmount;
            data.ValidBetAmount = source.ValidBetAmount;
            data.SettleAmount = source.SettleAmount;
            data.Commission = source.Commission;
            data.ReopenTurnoverAdjustment = data.Turnover;
            data.ReopenedOn = systemDateTimeOffset;
            data.Turnover = source.Turnover;
            data.WinLoss = source.CompanyWinLoss;
            data.ClientStatus = (short)source.ClientStatus;

            _ = _context.Update(data);

            _ = await _context
                        .SaveChangesAsync(
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            var recordId = await _serialNumberGenerator
                                    .GenerateAsync(
                                        cancellationToken: cancellationToken)
                                    .ConfigureAwait(false);

            var record = new RoundOperationRecord()
            {
                Id = recordId,
                RoundId = roundId,
                OperatorId = source.OperatorId,
                OldData = JsonSerializer.Serialize(oldData),
                NewData = JsonSerializer.Serialize(data),
                CreatedOn = systemDateTimeOffset,
                Remark = source.Remark
            };

            _ = await _context
                        .RoundOperationRecords
                        .AddAsync(
                            entity: record,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            _ = await _context
                        .SaveChangesAsync(
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            await transaction
                    .CommitAsync(
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await transaction
                    .RollbackAsync(
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            _logger.LogError(exception: ex, "An exception occurs when adjusting the closed game record.({roundId})", roundId);
        }
    }

    public async ValueTask<bool> ExistAsync(
        Guid roundId,
        CancellationToken cancellationToken = default)
        => await _context.Rounds.AsQueryable()
        .AnyAsync(
            predicate: x => x.Id == roundId,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false);

    public ValueTask<GameRoundInfo?> GetAsync(
        Guid roundId,
        CancellationToken cancellationToken = default)
        => (from round in _context.Rounds.AsQueryable()
            join roundDetail in _context.RoundDetails.AsQueryable()
            on round.Id equals roundDetail.RoundId
            into roundDetailData
            from data in roundDetailData.DefaultIfEmpty()
            where round.Id == roundId
            select new GameRoundInfo(
                round.Id,
                round.ExternalRoundId,
                round.GameProviderId,
                round.PlayerCode,
                round.GameId,
                round.CurrencyCode,
                (GameRoundStatus)round.Status,
                round.GameProviderConfigurationId,
                round.WalletId,
                round.CreatedOn,
                round.ClosedOn,
                round.UpdatedOn,
                round.BrandId,
                round.ExternalCreatedOn,
                round.ExternalClosedOn,
                round.BetAmount,
                round.ValidBetAmount,
                round.SettleAmount,
                round.Commission,
                round.Turnover,
                round.WinLoss,
                round.TieBetAdjustment,
                (GameRoundClientStatus)round.ClientStatus,
                round.ReopenTurnoverAdjustment,
                round.ReopenedOn,
                round.EffectiveBettingPercentage,
                data.AdditionalData
                ))
        .ToAsyncEnumerable()
        .FirstOrDefaultAsync(cancellationToken);

    public ValueTask<GameRoundInfo?> GetAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        CancellationToken cancellationToken = default)
        => (from round in _context.Rounds.AsQueryable()
            join roundDetail in _context.RoundDetails.AsQueryable()
            on round.Id equals roundDetail.RoundId
            into roundDetailData
            from data in roundDetailData.DefaultIfEmpty()
            where round.ExternalRoundId == externalRoundId
            && round.GameProviderId == gameProviderId
            && round.GameId == gameId
            && round.WalletId == walletId
            select new GameRoundInfo(
                round.Id,
                round.ExternalRoundId,
                round.GameProviderId,
                round.PlayerCode,
                round.GameId,
                round.CurrencyCode,
                (GameRoundStatus)round.Status,
                round.GameProviderConfigurationId,
                round.WalletId,
                round.CreatedOn,
                round.ClosedOn,
                round.UpdatedOn,
                round.BrandId,
                round.ExternalCreatedOn,
                round.ExternalClosedOn,
                round.BetAmount,
                round.ValidBetAmount,
                round.SettleAmount,
                round.Commission,
                round.Turnover,
                round.WinLoss,
                round.TieBetAdjustment,
                (GameRoundClientStatus)round.ClientStatus,
                round.ReopenTurnoverAdjustment,
                round.ReopenedOn,
                round.EffectiveBettingPercentage,
                data.AdditionalData
                ))
        .ToAsyncEnumerable()
        .FirstOrDefaultAsync(cancellationToken);

    public IAsyncEnumerable<Guid> GetListByRoundIdAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId)
        => _context.Rounds
            .AsQueryable()
            .Where(x => x.ExternalRoundId == externalRoundId
                    && x.GameProviderId == gameProviderId
                    && x.GameId == gameId)
            .Select(x => x.Id)
            .ToAsyncEnumerable();

    public async ValueTask<GameRoundDetailInfo?> GetRoundDetailAsync(
            Guid roundId,
        CancellationToken cancellationToken = default)
        => await _context.RoundDetails
            .AsQueryable()
            .Where(x => x.RoundId == roundId)
            .Select(x => new GameRoundDetailInfo(x.RoundId, x.AdditionalData))
            .FirstOrDefaultAsync(cancellationToken);

    public async ValueTask<Guid?> GetRoundIdAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        CancellationToken cancellationToken = default)
        => await _context.Rounds.AsQueryable()
        .Where(x => x.ExternalRoundId == externalRoundId
            && x.GameProviderId == gameProviderId
            && x.GameId == gameId
            && x.WalletId == walletId)
        .Select(x => (Guid?)x.Id)
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);

    public async ValueTask<GameRoundStatus> GetStatusAsync(
        Guid roundId,
        CancellationToken cancellationToken = default)
        => await _context.Rounds.AsQueryable()
        .Where(x => x.Id == roundId)
        .Select(x => (GameRoundStatus)x.Status)
        .SingleAsync(cancellationToken)
        .ConfigureAwait(false);

    public async ValueTask ManualCloseRoundAsync(
        Guid roundId,
        ManualCloseRound source,
        CancellationToken cancellationToken = default)
    {
        var data = await _context.Rounds
            .Where(x => x.Id == roundId)
            .FirstAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var oldData = data;

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var systemDateTimeOffset = DateTime.UtcNow;
            data.Status = (short)GameRoundStatus.Closed;
            data.UpdatedOn = systemDateTimeOffset;
            data.ClosedOn = systemDateTimeOffset;
            data.ExternalClosedOn = source.ExternalClosedOn.UtcDateTime;
            data.BetAmount = source.BetAmount;
            data.ValidBetAmount = source.ValidBetAmount;
            data.SettleAmount = source.SettleAmount;
            data.Commission = source.Commission;
            data.Turnover = source.Turnover;
            data.WinLoss = source.CompanyWinLoss;
            data.ClientStatus = (short)source.ClientStatus;

            _ = _context.Update(data);

            _ = await _context
                        .SaveChangesAsync(
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            var recordId = await _serialNumberGenerator
                                    .GenerateAsync(
                                        cancellationToken: cancellationToken)
                                    .ConfigureAwait(false);

            var record = new RoundOperationRecord()
            {
                Id = recordId,
                RoundId = roundId,
                OperatorId = source.OperatorId,
                OldData = JsonSerializer.Serialize(oldData),
                NewData = JsonSerializer.Serialize(data),
                CreatedOn = systemDateTimeOffset,
                Remark = source.Remark
            };

            _ = await _context
                        .RoundOperationRecords
                        .AddAsync(
                            entity: record,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            _ = await _context
                        .SaveChangesAsync(
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

            await transaction
                    .CommitAsync(
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await transaction
                    .RollbackAsync(
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            _logger.LogError(exception: ex, "An exception occurs when closing the game record.({roundId})", roundId);
        }
    }

    public async ValueTask<Guid?> NewRoundAsync(
        NewRound round,
        CancellationToken cancellationToken = default)
    {
        var roundId = await _serialNumberGenerator.GenerateAsync(
                cancellationToken: cancellationToken).ConfigureAwait(false);

        var data = new Round()
        {
            Id = roundId,
            ExternalRoundId = round.ExternalRoundId,
            GameProviderId = round.GameProviderId,
            PlayerCode = round.PlayerCode,
            GameId = round.GameId,
            CurrencyCode = round.CurrencyCode,
            Status = (short)GameRoundStatus.New,
            GameProviderConfigurationId = round.GameProviderConfigurationId,
            WalletId = round.WalletId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            BrandId = round.BrandId,
            ExternalCreatedOn = DateTime.SpecifyKind(round.ExternalCreatedOn.UtcDateTime, DateTimeKind.Utc),
            ClientStatus = (short)GameRoundClientStatus.Open
        };
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _ = _context.Rounds.Add(data);

            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var getRound = await _context.Rounds.FirstAsync(
                predicate: x => x.Id == roundId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            _ = _context.RoundDetails.Add(new()
            {
                RoundId = getRound.Id,
                AdditionalData = string.Empty
            });
            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return roundId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{DateTime.Now.ToString(\"o\")} Data: {round} Message:添加新局出現例外", DateTime.Now.ToString("o"), round);
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            return null;
        }
    }

    public async ValueTask UpdateClientStatusAsync(
        Guid roundId,
        GameRoundClientStatus status,
        CancellationToken cancellationToken = default)
    {
        var data = await _context.Rounds.SingleAsync(
            predicate: x => x.Id == roundId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        data.UpdatedOn = DateTime.UtcNow;
        data.ClientStatus = (short)status;
        _ = _context.Rounds.Update(data);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UpdateRoundClosedAsync(
        Guid roundId,
        UpdateRoundClosed source,
        CancellationToken cancellationToken = default)
    {
        var round = await _context.Rounds.SingleAsync(
           predicate: x => x.Id == roundId,
           cancellationToken: cancellationToken).ConfigureAwait(false);

        round.Status = (short)GameRoundStatus.Closed;
        round.ClosedOn = DateTime.UtcNow;
        round.UpdatedOn = DateTime.UtcNow;
        round.ExternalClosedOn = DateTime.SpecifyKind(source.ExternalClosedOn.UtcDateTime, DateTimeKind.Utc);
        round.BetAmount = source.BetAmount;
        round.ValidBetAmount = source.ValidBetAmount;
        round.SettleAmount = source.SettleAmount;
        round.Commission = source.Commission;
        round.Turnover = source.Turnover;
        round.WinLoss = -source.PlayerWinLoss;
        round.TieBetAdjustment = 0;
        round.ClientStatus = (short)source.ClientStatus;
        _ = _context.Rounds.Update(round);

        var roundDetail = await _context.RoundDetails.SingleAsync(
            predicate: x => x.RoundId == roundId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        roundDetail.AdditionalData = source.AdditionalData;
        _ = _context.RoundDetails.Update(roundDetail);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UpdateRoundDetailAsync(
        Guid roundId,
        string additionalData,
        CancellationToken cancellationToken = default)
    {
        var roundDetail = await _context.RoundDetails.SingleAsync(
            predicate: x => x.RoundId == roundId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        roundDetail.AdditionalData = additionalData;
        _ = _context.RoundDetails.Update(roundDetail);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UpdateRoundOpenAsync(
        Guid roundId,
        UpdateRoundOpen source,
        CancellationToken cancellationToken = default)
    {
        var round = await _context.Rounds.SingleAsync(
            predicate: x => x.Id == roundId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        round.Status = (short)GameRoundStatus.Open;
        round.UpdatedOn = DateTime.UtcNow;
        round.BetAmount = source.BetAmount;
        round.ValidBetAmount = source.ValidBetAmount;
        round.Commission = source.Commission;
        round.Turnover = source.Turnover;
        _ = _context.Rounds.Update(round);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UpdateRoundOpenSettleAsync(
        Guid roundId,
        UpdateRoundOpenSettle source,
        CancellationToken cancellationToken = default)
    {
        var round = await _context.Rounds.SingleAsync(
            predicate: x => x.Id == roundId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        round.Status = (short)GameRoundStatus.Open;
        round.UpdatedOn = DateTime.UtcNow;
        round.BetAmount = source.BetAmount;
        round.ValidBetAmount = source.ValidBetAmount;
        round.SettleAmount = source.SettleAmount;
        round.Commission = source.Commission;
        round.Turnover = source.Turnover;
        round.WinLoss = -source.PlayerWinLoss;
        _ = _context.Rounds.Update(round);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UpdateRoundReopenAsync(
        Guid roundId,
        UpdateRoundReopen source,
        CancellationToken cancellationToken = default)
    {
        var round = await _context.Rounds.SingleAsync(
                predicate: x => x.Id == roundId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

        round.Status = (short)source.Status;
        round.UpdatedOn = DateTime.UtcNow;
        round.ExternalClosedOn = (source.Status == GameRoundStatus.Closed) ? DateTime.SpecifyKind(source.ExternalClosedOn!.Value.UtcDateTime, DateTimeKind.Utc) : null;
        round.BetAmount = source.BetAmount;
        round.ValidBetAmount = source.ValidBetAmount;
        round.SettleAmount = source.SettleAmount;
        round.Commission = source.Commission;
        round.Turnover = source.Turnover;
        round.WinLoss = -source.PlayerWinLoss;
        round.TieBetAdjustment = 0;
        round.ClientStatus = (short)source.ClientStatus;
        round.ReopenTurnoverAdjustment = round.Turnover ?? 0;
        round.ReopenedOn = DateTime.UtcNow;
        _ = _context.Rounds.Update(round);

        var roundDetail = await _context.RoundDetails.SingleAsync(
            predicate: x => x.RoundId == roundId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        roundDetail.AdditionalData = source.AdditionalData;
        _ = _context.RoundDetails.Update(roundDetail);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UpdateStatusAsync(
        Guid roundId,
        GameRoundStatus status,
        DateTimeOffset? externalClosedOn = null,
        CancellationToken cancellationToken = default)
    {
        var data = await _context.Rounds.SingleAsync(
            predicate: x => x.Id == roundId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        data.UpdatedOn = DateTime.UtcNow;
        data.Status = (short)status;
        data.ExternalClosedOn = externalClosedOn.HasValue ? externalClosedOn.Value.UtcDateTime : data.ExternalClosedOn;
        _ = _context.Rounds.Update(data);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
