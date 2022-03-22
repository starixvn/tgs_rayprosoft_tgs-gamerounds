using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateRoundGetService : IGetService<UpdateRoundQuery, UpdateRoundResult>
    {
        private readonly IGameRoundService _gameRoundCacheService;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly IGameRoundRepository _gameRoundRepository;

        public UpdateRoundGetService(
          IGameRoundService gameRoundCacheService,
          IGameRoundRecordRepository gameRoundRecordRepository,
          IGameRoundRepository gameRoundRepository)
        {
            _gameRoundCacheService = gameRoundCacheService ?? throw new ArgumentNullException(nameof(gameRoundCacheService));
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
        }

        public async ValueTask<UpdateRoundResult> GetAsync(UpdateRoundQuery query, CancellationToken cancellationToken = default)
        {
            if (!await _gameRoundCacheService.ExistAsync(query.RoundId).ConfigureAwait(false))
            {
                return new(Success: false);
            }

            await _gameRoundRepository.UpdateRoundAsync(new(
                query.RoundId,
                query.Status,
                query.ClosedOn,
                query.ExternalClosedOn,
                query.BetAmount,
                query.ValidBetAmount,
                query.SettleAmount,
                query.Commission,
                query.Turnover,
                query.WinLoss,
                query.TieBetAdjustment,
                query.ClientStatus,
                query.ReopenTurnoverAdjustment,
                query.ReopenedOn,
                query.AdditionalData)).ConfigureAwait(false);

            var round = await _gameRoundRepository.GetAsync(query.RoundId).ConfigureAwait(false);

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

            return new(Success: true);
        }
    }
}
