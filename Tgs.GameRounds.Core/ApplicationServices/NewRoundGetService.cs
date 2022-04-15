using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class NewRoundGetService : IGetService<NewRoundQuery, Guid>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;

        public NewRoundGetService(
          IGameRoundRecordRepository gameRoundRecordRepository,
          IGameRound gameRound)
        {
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
        }

        public async ValueTask<Guid> GetAsync(NewRoundQuery query, CancellationToken cancellationToken = default)
        {
            var roundId = await _gameRound.GetRoundIdAsync(query.GameProviderId, query.ExternalRoundId, query.GameId, query.WalletId).ConfigureAwait(false);
            if (roundId.HasValue)
            {
                return roundId.Value;
            }

            if (!await _gameRound.LockAsync(query.GameProviderId, query.ExternalRoundId, query.GameId, query.WalletId).ConfigureAwait(false))
            {
                throw new Exception("This GameRound generated order is currently locked.");
            }

            var newRoundId = await _gameRound.NewRoundAsync(
                new NewRound(
                    query.ExternalRoundId,
                    query.GameProviderId,
                    query.PlayerCode,
                    query.GameId,
                    query.CurrencyCode,
                    query.GameProviderConfigurationId,
                    query.WalletId,
                    query.BrandId,
                    query.ExternalCreatedOn)).ConfigureAwait(false);

            await _gameRound.UnLockAsync(query.GameProviderId, query.ExternalRoundId, query.GameId, query.WalletId).ConfigureAwait(false);

            if (!newRoundId.HasValue)
            {
                throw new Exception("This GameRound failed to create an order.");
            }
            else
            {
                var round = await _gameRound.GetAsync(newRoundId.Value).ConfigureAwait(false);

                if (round is not null)
                {
                    await _gameRoundRecordRepository.AddAsync(new(
                        round.RoundId,
                        round.ExternalRoundId,
                        round.GameProviderId,
                        round.PlayerCode,
                        round.GameId,
                        round.CurrencyCode,
                        round.Status,
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
                        round.ClientStatus,
                        round.ReopenTurnoverAdjustment,
                        round.ReopenedOn,
                        round.EffectiveBettingPercentage,
                        round.AdditionalData)).ConfigureAwait(false);
                }
            }

            return newRoundId.Value;
        }
    }
}
