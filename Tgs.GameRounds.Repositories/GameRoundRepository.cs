using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories;

internal class GameRoundRepository : IGameRoundRepository
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
        var roundId = await _serialNumberGenerator
            .GenerateAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

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

    public async ValueTask AdjustClosedRoundAsync(
        Guid roundId,
        AdjustClosedRound source,
        CancellationToken cancellationToken = default)
    {
        var data = await _context
            .Rounds
            .FirstOrDefaultAsync(
                predicate: x => x.Id == roundId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This round does not exist.({roundId})");

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
        Guid id,
        CancellationToken cancellationToken = default)
        => await _context.Rounds.AsQueryable()
        .AnyAsync(
            predicate: x => x.Id == id,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false);

    public ValueTask<GameRoundInfo?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _context.Rounds.AsQueryable()
        .Where(x => x.Id == id)
        .Select(round => new GameRoundInfo(
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
            round.BetAmount.GetValueOrDefault(0),
            round.ValidBetAmount.GetValueOrDefault(0),
            round.SettleAmount.GetValueOrDefault(0),
            round.Commission.GetValueOrDefault(0),
            round.Turnover.GetValueOrDefault(0),
            round.WinLoss.GetValueOrDefault(0),
            round.TieBetAdjustment.GetValueOrDefault(0),
            (GameRoundClientStatus)round.ClientStatus,
            round.ReopenTurnoverAdjustment,
            round.ReopenedOn,
            round.EffectiveBettingPercentage))
        .ToAsyncEnumerable()
        .FirstOrDefaultAsync(cancellationToken);

    public ValueTask<GameRoundInfo?> GetAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        CancellationToken cancellationToken = default)
        => _context.Rounds.AsQueryable()
        .Where(x => x.GameProviderId == gameProviderId &&
        x.ExternalRoundId == externalRoundId &&
        x.GameId == gameId &&
        x.WalletId == walletId)
        .Select(round => new GameRoundInfo(
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
            round.BetAmount.GetValueOrDefault(0),
            round.ValidBetAmount.GetValueOrDefault(0),
            round.SettleAmount.GetValueOrDefault(0),
            round.Commission.GetValueOrDefault(0),
            round.Turnover.GetValueOrDefault(0),
            round.WinLoss.GetValueOrDefault(0),
            round.TieBetAdjustment.GetValueOrDefault(0),
            (GameRoundClientStatus)round.ClientStatus,
            round.ReopenTurnoverAdjustment,
            round.ReopenedOn,
            round.EffectiveBettingPercentage))
        .ToAsyncEnumerable()
        .FirstOrDefaultAsync(cancellationToken);

    public ValueTask<GameRoundInfo?> GetAsync(
        Guid id,
        DateTimeOffset externalCreatedOn,
        CancellationToken cancellationToken = default)

        => _context.Rounds.AsQueryable()
        .Where(x =>
            x.CreatedOn >= externalCreatedOn.UtcDateTime &&
            x.Id == id)
        .Select(round => new GameRoundInfo(
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
            round.BetAmount.GetValueOrDefault(0),
            round.ValidBetAmount.GetValueOrDefault(0),
            round.SettleAmount.GetValueOrDefault(0),
            round.Commission.GetValueOrDefault(0),
            round.Turnover.GetValueOrDefault(0),
            round.WinLoss.GetValueOrDefault(0),
            round.TieBetAdjustment.GetValueOrDefault(0),
            (GameRoundClientStatus)round.ClientStatus,
            round.ReopenTurnoverAdjustment,
            round.ReopenedOn,
            round.EffectiveBettingPercentage))
        .ToAsyncEnumerable()
        .FirstOrDefaultAsync(cancellationToken);

    public ValueTask<GameRoundInfo?> GetAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        DateTimeOffset externalCreatedOn,
        CancellationToken cancellationToken = default)
        => _context.Rounds.AsQueryable()
        .Where(x =>
            x.CreatedOn >= externalCreatedOn.UtcDateTime &&
            x.GameProviderId == gameProviderId &&
            x.ExternalRoundId == externalRoundId &&
            x.GameId == gameId &&
            x.WalletId == walletId)
        .Select(round => new GameRoundInfo(
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
            round.BetAmount.GetValueOrDefault(0),
            round.ValidBetAmount.GetValueOrDefault(0),
            round.SettleAmount.GetValueOrDefault(0),
            round.Commission.GetValueOrDefault(0),
            round.Turnover.GetValueOrDefault(0),
            round.WinLoss.GetValueOrDefault(0),
            round.TieBetAdjustment.GetValueOrDefault(0),
            (GameRoundClientStatus)round.ClientStatus,
            round.ReopenTurnoverAdjustment,
            round.ReopenedOn,
            round.EffectiveBettingPercentage))
        .ToAsyncEnumerable()
        .FirstOrDefaultAsync(cancellationToken);

    public async ValueTask<GameRoundDetailInfo?> GetRoundDetailAsync(
        Guid roundId,
        CancellationToken cancellationToken = default)
        => await _context.RoundDetails
            .AsQueryable()
            .Where(x => x.RoundId == roundId)
            .Select(x => new GameRoundDetailInfo(x.RoundId, x.AdditionalData))
            .FirstOrDefaultAsync(cancellationToken);

    public ValueTask<Guid?> GetRoundIdAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        CancellationToken cancellationToken = default)
        => _context.Rounds.AsQueryable()
        .Where(x => x.ExternalRoundId == externalRoundId
            && x.GameProviderId == gameProviderId
            && x.GameId == gameId
            && x.WalletId == walletId)
        .Select(x => (Guid?)x.Id)
        .ToAsyncEnumerable()
        .FirstOrDefaultAsync(cancellationToken);

    public ValueTask<Guid?> GetRoundIdAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        DateTimeOffset externalCreatedOn,
        CancellationToken cancellationToken = default)
        => _context.Rounds.AsQueryable()
        .Where(x =>
            x.CreatedOn >= externalCreatedOn.UtcDateTime &&
            x.ExternalRoundId == externalRoundId &&
            x.GameProviderId == gameProviderId &&
            x.GameId == gameId &&
            x.WalletId == walletId)
        .Select(x => (Guid?)x.Id)
        .ToAsyncEnumerable()
        .FirstOrDefaultAsync(cancellationToken);

    public async ValueTask<GameRoundStatus> GetStatusAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await _context.Rounds.AsQueryable()
        .Where(x => x.Id == id)
        .Select(x => (GameRoundStatus)x.Status)
        .SingleAsync(cancellationToken)
        .ConfigureAwait(false);

    public async ValueTask ManualCloseRoundAsync(
        Guid id,
        ManualCloseRound source,
        CancellationToken cancellationToken = default)
    {
        var data = await _context
            .Rounds
            .FirstOrDefaultAsync(
                predicate: x => x.Id == id,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This round does not exist.({id})");

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
                RoundId = id,
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
            _logger.LogError(exception: ex, "An exception occurs when closing the game record.({roundId})", id);
        }
    }

    public async ValueTask UpdateRoundClosedAsync(
        Guid id,
        UpdateRoundClosed source,
        CancellationToken cancellationToken = default)
    {
        var round = await _context
            .Rounds
            .FirstOrDefaultAsync(
                predicate: x => x.Id == id,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException($"This round does not exist.({id})");

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

        var roundDetail = await _context
            .RoundDetails
            .FirstOrDefaultAsync(
                predicate: x => x.RoundId == id,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        if (roundDetail == null)
        {
            roundDetail = new RoundDetail
            {
                RoundId = id,
                AdditionalData = source.AdditionalData,
            };
            _ = _context.RoundDetails.Add(roundDetail);
        }
        else
        {
            roundDetail.AdditionalData = source.AdditionalData;
            _ = _context.RoundDetails.Update(roundDetail);
        }

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UpdateRoundDetailAsync(
        Guid id,
        string additionalData,
        CancellationToken cancellationToken = default)
    {
        var roundDetail = await _context
            .RoundDetails
            .FirstOrDefaultAsync(
                predicate: x => x.RoundId == id,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This round detail does not exist.({id})");

        roundDetail.AdditionalData = additionalData;
        _ = _context.RoundDetails.Update(roundDetail);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UpdateRoundOpenAsync(
        Guid id,
        UpdateRoundOpen source,
        CancellationToken cancellationToken = default)
    {
        var round = await _context
            .Rounds
            .FirstOrDefaultAsync(
                predicate: x => x.Id == id,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException($"This round does not exist.({id})");

        round.Status = (short)GameRoundStatus.Open;
        round.UpdatedOn = DateTime.UtcNow;
        round.BetAmount = source.BetAmount;
        round.ValidBetAmount = source.ValidBetAmount;
        round.Commission = source.Commission;
        round.Turnover = source.Turnover;
        _ = _context.Rounds.Update(round);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UpdateRoundReopenAsync(
        Guid id,
        UpdateRoundReopen source,
        CancellationToken cancellationToken = default)
    {
        var round = await _context
            .Rounds
            .FirstOrDefaultAsync(
                predicate: x => x.Id == id,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException($"This round does not exist.({id})");

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
            predicate: x => x.RoundId == id,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        roundDetail.AdditionalData = source.AdditionalData;
        _ = _context.RoundDetails.Update(roundDetail);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
