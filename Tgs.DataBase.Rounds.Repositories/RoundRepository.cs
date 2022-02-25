using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tgs.DataBase.Rounds.Core;
using Tgs.DataBase.Rounds.Core.Models;
using Tgs.DataBase.Rounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.DataBase.Rounds.Repositories
{
    internal class RoundRepository : IRoundRepository
    {
        private readonly BetContext _context;
        private readonly ILogger<RoundRepository> _logger;
        private readonly ISerialNumberGenerator _serialNumberGenerator;

        public RoundRepository(
            BetContext context,
            ILogger<RoundRepository> logger,
            ISerialNumberGenerator serialNumberGenerator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
        }

        public async ValueTask ClosedRoundAsync(ClosedRound round)
        {
            var data = await _context.Rounds.SingleAsync(x => x.Id == round.RoundId).ConfigureAwait(false);
            data.Status = (short)RoundStatus.Closed;
            data.UpdatedOn = DateTime.UtcNow;
            data.ClosedOn = DateTime.UtcNow;
            data.ExternalClosedOn = DateTime.SpecifyKind(round.ExternalClosedOn.UtcDateTime, DateTimeKind.Utc);
            data.BetAmount = round.BetAmount;
            data.ValidBetAmount = round.ValidBetAmount;
            data.SettleAmount = round.SettleAmount;
            data.Commission = round.Commission;
            data.Turnover = round.Turnover;
            data.WinLoss = round.WinLoss;
            data.ClientStatus = (short)round.ClientStatus;
            _ = _context.Rounds.Update(data);
            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public ValueTask<RoundInfo?> GetAsync(Guid roundId)
            => (from round in _context.Rounds.AsQueryable()
                join roundDetail in _context.RoundDetails.AsQueryable()
                on round.Id equals roundDetail.RoundId
                into roundDetailData
                from data in roundDetailData.DefaultIfEmpty()
                where round.Id == roundId
                select new RoundInfo(
                    round.Id,
                    round.ExternalRoundId,
                    round.GameProviderId,
                    round.PlayerCode,
                    round.GameId,
                    round.CurrencyCode,
                    (RoundStatus)round.Status,
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
                    (RoundClientStatus)round.ClientStatus,
                    round.ReopenTurnoverAdjustment,
                    round.ReopenedOn,
                    round.EffectiveBettingPercentage,
                    data.AdditionalData
                    ))
            .ToAsyncEnumerable()
            .FirstOrDefaultAsync();

        public ValueTask<RoundInfo?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
            => (from round in _context.Rounds.AsQueryable()
                join roundDetail in _context.RoundDetails.AsQueryable()
                on round.Id equals roundDetail.RoundId
                into roundDetailData
                from data in roundDetailData.DefaultIfEmpty()
                where round.GameProviderId == gameProviderId
                && round.ExternalRoundId == externalRoundId
                && round.GameId == gameId
                && round.WalletId == walletId
                select new RoundInfo(
                    round.Id,
                    round.ExternalRoundId,
                    round.GameProviderId,
                    round.PlayerCode,
                    round.GameId,
                    round.CurrencyCode,
                    (RoundStatus)round.Status,
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
                    (RoundClientStatus)round.ClientStatus,
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
                Status = (short)RoundStatus.New,
                GameProviderConfigurationId = round.GameProviderConfigurationId,
                WalletId = round.WalletId,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                BrandId = round.BrandId,
                ExternalCreatedOn = DateTime.SpecifyKind(round.ExternalCreatedOn.UtcDateTime, DateTimeKind.Utc),
                ClientStatus = (short)RoundClientStatus.Open
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
                    new EventId(10000, nameof(RoundRepository)),
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

        public async ValueTask OpenRoundAsync(OpenRound round)
        {
            var data = await _context.Rounds.SingleAsync(x => x.Id == round.RoundId).ConfigureAwait(false);
            data.Status = (short)RoundStatus.Open;
            data.UpdatedOn = DateTime.UtcNow;
            data.BetAmount = round.BetAmount;
            data.ValidBetAmount = round.ValidBetAmount;
            _ = _context.Rounds.Update(data);
            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask ReopenedRoundAsync(ReopenedRound round)
        {
            var data = await _context.Rounds.SingleAsync(x => x.Id == round.RoundId).ConfigureAwait(false);
            data.Status = (short)RoundStatus.Closed;
            data.UpdatedOn = DateTime.UtcNow;
            data.ClosedOn = DateTime.UtcNow;
            data.BetAmount = round.BetAmount;
            data.ValidBetAmount = round.ValidBetAmount;
            data.SettleAmount = round.SettleAmount;
            data.Commission = round.Commission;
            data.Turnover = round.Turnover;
            data.WinLoss = round.WinLoss;
            data.ClientStatus = (short)round.ClientStatus;
            data.ReopenedOn = DateTime.UtcNow;
            data.ReopenTurnoverAdjustment = data.Turnover;
            _ = _context.Rounds.Update(data);
            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask TurnoverRoundAsync(TurnoverRound round)
        {
            var data = await _context.Rounds.SingleAsync(x => x.Id == round.RoundId).ConfigureAwait(false);
            data.UpdatedOn = DateTime.UtcNow;
            data.Turnover = round.Turnover;
            _ = _context.Rounds.Update(data);
            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask UpdateRoundAsync(UpdateRound round)
        {
            var data = await _context.Rounds.SingleAsync(x => x.Id == round.RoundId).ConfigureAwait(false);
            data.Status = (short)RoundStatus.Open;
            data.UpdatedOn = DateTime.UtcNow;
            data.BetAmount = round.BetAmount;
            data.ValidBetAmount = round.ValidBetAmount;
            data.SettleAmount = round.SettleAmount;
            data.Commission = round.Commission;
            data.Turnover = round.Turnover;
            data.WinLoss = round.WinLoss;
            _ = _context.Rounds.Update(data);
            _ = await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
