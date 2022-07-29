using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories
{
    internal class GameRoundRepository : IGameRound
    {
        private readonly BetContext _context;
        private readonly IGameRound _gameRound;
        private readonly ILogger<GameRoundRepository> _logger;
        private readonly ISerialNumberGenerator _serialNumberGenerator;

        public GameRoundRepository(
            BetContext context,
            IGameRound gameRound,
            ILogger<GameRoundRepository> logger,
            ISerialNumberGenerator serialNumberGenerator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
        }

        public async ValueTask<Guid?> AddRoundAsync(AddRound source)
        {
            var roundId = await _serialNumberGenerator.GenerateAsync().ConfigureAwait(false);
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
                ExternalClosedOn = (source.ExternalClosedOn.HasValue) ? DateTime.SpecifyKind(source.ExternalClosedOn.Value.UtcDateTime, DateTimeKind.Utc) : null,
                BetAmount = source.BetAmount,
                ValidBetAmount = source.ValidBetAmount,
                SettleAmount = source.SettleAmount,
                Commission = source.Commission,
                Turnover = source.Turnover,
                WinLoss = source.PlayerWinLoss.HasValue ? -(source.PlayerWinLoss) : null,
                ClientStatus = (short)source.ClientStatus
            };
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _ = _context.Rounds.Add(round);

                _ = await _context.SaveChangesAsync().ConfigureAwait(false);

                var getRound = await _context.Rounds.FirstAsync(x => x.Id == roundId).ConfigureAwait(false);

                _ = _context.RoundDetails.Add(new()
                {
                    RoundId = getRound.Id,
                    AdditionalData = source.AdditionalData ?? string.Empty,
                });
                _ = await _context.SaveChangesAsync().ConfigureAwait(false);

                await transaction.CommitAsync().ConfigureAwait(false);
                return roundId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{DateTime.Now.ToString(\"o\")} Data: {round} Message:添加新局出現例外", DateTime.Now.ToString("o"), round);
                await transaction.RollbackAsync().ConfigureAwait(false);
                return null;
            }
        }

        public async ValueTask<bool> ExistAsync(Guid roundId)
            => (await _context.Rounds.AsQueryable()
            .Where(x => x.Id == roundId)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false)).HasValue;

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
            .FirstOrDefaultAsync();

        public async ValueTask<GameRoundDetailInfo?> GetRoundDetailAsync(Guid roundId)
            => await _context.RoundDetails
                .AsQueryable()
                .Where(x => x.RoundId == roundId)
                .Select(x => new GameRoundDetailInfo(x.RoundId, x.AdditionalData))
                .FirstOrDefaultAsync();

        public async ValueTask<Guid?> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
            => await _context.Rounds.AsQueryable()
            .Where(x => x.ExternalRoundId == externalRoundId
                && x.GameProviderId == gameProviderId
                && x.GameId == gameId
                && x.WalletId == walletId)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        public ValueTask<bool> LockAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
            => _gameRound.LockAsync(gameProviderId, externalRoundId, gameId, walletId);

        public async ValueTask<Guid?> NewRoundAsync(NewRound round)
        {
            var roundId = await _serialNumberGenerator.GenerateAsync().ConfigureAwait(false);

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
                _logger.LogError(ex, "LogOn:{DateTime.Now.ToString(\"o\")} Data: {round} Message:添加新局出現例外", DateTime.Now.ToString("o"), round);
                await transaction.RollbackAsync().ConfigureAwait(false);
                return null;
            }
        }

        public ValueTask UnLockAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
            => _gameRound.UnLockAsync(gameProviderId, externalRoundId, gameId, walletId);

        public async ValueTask UpdateClientStatusAsync(Guid roundId, GameRoundClientStatus status)
        {
            var data = await _context.Rounds.SingleAsync(x => x.Id == roundId).ConfigureAwait(false);

            data.UpdatedOn = DateTime.UtcNow;
            data.ClientStatus = (short)status;

            _ = _context.Rounds.Update(data);
            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask UpdateRoundClosedAsync(UpdateRoundClosed source)
        {
            var round = await _context.Rounds.SingleAsync(x => x.Id == source.RoundId).ConfigureAwait(false);
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
            var roundDetail = await _context.RoundDetails.SingleAsync(x => x.RoundId == source.RoundId).ConfigureAwait(false);
            roundDetail.AdditionalData = source.AdditionalData;
            _ = _context.Rounds.Update(round);
            _ = _context.RoundDetails.Update(roundDetail);

            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask UpdateRoundDetailAsync(Guid roundId, string additionalData)
        {
            var roundDetail = await _context.RoundDetails.SingleAsync(x => x.RoundId == roundId).ConfigureAwait(false);
            roundDetail.AdditionalData = additionalData;
            _ = _context.RoundDetails.Update(roundDetail);

            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask UpdateRoundOpenAsync(UpdateRoundOpen source)
        {
            var round = await _context.Rounds.SingleAsync(x => x.Id == source.RoundId).ConfigureAwait(false);
            round.Status = (short)GameRoundStatus.Open;
            round.UpdatedOn = DateTime.UtcNow;
            round.BetAmount = source.BetAmount;
            round.ValidBetAmount = source.ValidBetAmount;
            round.Commission = source.Commission;
            round.Turnover = source.Turnover;

            _ = _context.Rounds.Update(round);
            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask UpdateRoundOpenSettleAsync(UpdateRoundOpenSettle source)
        {
            var round = await _context.Rounds.SingleAsync(x => x.Id == source.RoundId).ConfigureAwait(false);
            round.Status = (short)GameRoundStatus.Open;
            round.UpdatedOn = DateTime.UtcNow;
            round.BetAmount = source.BetAmount;
            round.ValidBetAmount = source.ValidBetAmount;
            round.SettleAmount = source.SettleAmount;
            round.Commission = source.Commission;
            round.Turnover = source.Turnover;
            round.WinLoss = -source.PlayerWinLoss;

            _ = _context.Rounds.Update(round);
            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask UpdateRoundReopenAsync(UpdateRoundReopen source)
        {
            var round = await _context.Rounds.SingleAsync(x => x.Id == source.RoundId).ConfigureAwait(false);
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
            var roundDetail = await _context.RoundDetails.SingleAsync(x => x.RoundId == source.RoundId).ConfigureAwait(false);
            roundDetail.AdditionalData = source.AdditionalData;
            _ = _context.Rounds.Update(round);
            _ = _context.RoundDetails.Update(roundDetail);

            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask UpdateStatusAsync(Guid roundId, GameRoundStatus status, DateTimeOffset? externalClosedOn = null)
        {
            var data = await _context.Rounds.SingleAsync(x => x.Id == roundId).ConfigureAwait(false);

            data.UpdatedOn = DateTime.UtcNow;
            data.Status = (short)status;
            data.ExternalClosedOn = (externalClosedOn.HasValue) ? externalClosedOn.Value.UtcDateTime : data.ExternalClosedOn;

            _ = _context.Rounds.Update(data);
            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
