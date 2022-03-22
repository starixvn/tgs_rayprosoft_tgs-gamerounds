using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class GetGameRoundByExternalGameGetService : IGetService<GetGameRoundByExternalGameQuery, GetGameRoundByExternalGameResult?>
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameRoundRepository _gameRoundRepository;

        public GetGameRoundByExternalGameGetService(
            IGameRepository gameRepository,
            IGameRoundRepository gameRoundRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
        }

        public async ValueTask<GetGameRoundByExternalGameResult?> GetAsync(GetGameRoundByExternalGameQuery query, CancellationToken cancellationToken = default)
        {
            var gameId = await _gameRepository.GetGameIdAsync(query.GameProviderId, query.ExternalGameId).ConfigureAwait(false);
            if (gameId == null || !gameId.HasValue)
            {
                return null;
            }

            var gameRoundInfo = await _gameRoundRepository.GetAsync(query.GameProviderId, query.ExternalRoundId, gameId.Value, query.WalletId).ConfigureAwait(false);

            return gameRoundInfo != null ? new GetGameRoundByExternalGameResult(
                gameRoundInfo.RoundId,
                gameRoundInfo.ExternalRoundId,
                gameRoundInfo.GameProviderId,
                gameRoundInfo.PlayerCode,
                gameRoundInfo.GameId,
                gameRoundInfo.CurrencyCode,
                gameRoundInfo.Status,
                gameRoundInfo.GameProviderConfigurationId,
                gameRoundInfo.WalletId,
                gameRoundInfo.CreatedOn,
                gameRoundInfo.ClosedOn,
                gameRoundInfo.UpdatedOn,
                gameRoundInfo.BrandId,
                gameRoundInfo.ExternalCreatedOn,
                gameRoundInfo.ExternalClosedOn,
                gameRoundInfo.BetAmount,
                gameRoundInfo.ValidBetAmount,
                gameRoundInfo.SettleAmount,
                gameRoundInfo.Commission,
                gameRoundInfo.Turnover,
                gameRoundInfo.WinLoss,
                gameRoundInfo.TieBetAdjustment,
                gameRoundInfo.ClientStatus,
                gameRoundInfo.ReopenTurnoverAdjustment,
                gameRoundInfo.ReopenedOn,
                gameRoundInfo.EffectiveBettingPercentage,
                gameRoundInfo.AdditionalData) : null;
        }
    }
}
