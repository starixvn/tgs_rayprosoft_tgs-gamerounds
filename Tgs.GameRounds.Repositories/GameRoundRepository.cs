using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories
{
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

        public ValueTask<GameRoundInfo?> GetAsync(Guid roundId)
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
            .FirstOrDefaultAsync();

        public ValueTask<GameRoundInfo?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
            => (from round in _context.Rounds.AsQueryable()
                join roundDetail in _context.RoundDetails.AsQueryable()
                on round.Id equals roundDetail.RoundId
                into roundDetailData
                from data in roundDetailData.DefaultIfEmpty()
                where round.GameProviderId == gameProviderId
                && round.ExternalRoundId == externalRoundId
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
            .FirstOrDefaultAsync();

        public async ValueTask<Guid?> NewRoundAsync(NewRound round)
        {
            Guid roundId = await _serialNumberGenerator
                .GenerateAsync()
                .ConfigureAwait(false);

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

                _ = await _context.SaveChangesAsync().ConfigureAwait(false);

                var getRound = await _context.Rounds.FirstAsync(x => x.Id == roundId).ConfigureAwait(false);

                _ = _context.RoundDetails.Add(new()
                {
                    RoundId = getRound.Id,
                    AdditionalData = string.Empty
                });
                _ = await _context.SaveChangesAsync().ConfigureAwait(false);

                await transaction.CommitAsync().ConfigureAwait(false);
                return roundId;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                    new EventId(10000, nameof(GameRoundRepository)),
                    data,
                    ex,
                    (state, ex) => JsonSerializer.Serialize(new
                    {
                        Data = data,
                        Exception = ex!,
                        Message = "添加新局出現例外",
                        LogOn = DateTimeOffset.UtcNow.ToString("o")
                    }));
                await transaction.RollbackAsync().ConfigureAwait(false);
                return null;
            }
        }

        public async ValueTask UpdateRoundAsync(UpdateRound round)
        {
            var data = await _context.Rounds.SingleAsync(x => x.Id == round.RoundId).ConfigureAwait(false);
            data.Status = (short)round.Status;

            data.UpdatedOn = DateTime.UtcNow;
            data.ClosedOn = (round.ClosedOn.HasValue) ? DateTime.SpecifyKind(round.ClosedOn.Value.UtcDateTime, DateTimeKind.Utc) : null;
            data.ExternalClosedOn = (round.ExternalClosedOn.HasValue) ? DateTime.SpecifyKind(round.ExternalClosedOn.Value.UtcDateTime, DateTimeKind.Utc) : null;
            data.BetAmount = round.BetAmount;
            data.ValidBetAmount = round.ValidBetAmount;
            data.SettleAmount = round.SettleAmount;
            data.Commission = round.Commission;
            data.Turnover = round.Turnover;
            data.WinLoss = round.WinLoss;
            data.TieBetAdjustment = round.TieBetAdjustment;
            data.ClientStatus = (short)round.ClientStatus;
            data.ReopenTurnoverAdjustment = round.ReopenTurnoverAdjustment;
            data.ReopenedOn = (round.ReopenedOn.HasValue) ? DateTime.SpecifyKind(round.ReopenedOn.Value.UtcDateTime, DateTimeKind.Utc) : null;
            _ = _context.Rounds.Update(data);
            var detail = await _context.RoundDetails.FirstOrDefaultAsync(x => x.RoundId == round.RoundId).ConfigureAwait(false);
            if (detail == null)
            {
                detail = new RoundDetail()
                {
                    RoundId = round.RoundId,
                    AdditionalData = round.AdditionalData ?? string.Empty
                };
                _ = await _context.RoundDetails.AddAsync(detail).ConfigureAwait(false);
            }
            else
            {
                detail.AdditionalData = round.AdditionalData ?? string.Empty;
                _ = _context.RoundDetails.Update(detail);
            }

            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }

    }
}
