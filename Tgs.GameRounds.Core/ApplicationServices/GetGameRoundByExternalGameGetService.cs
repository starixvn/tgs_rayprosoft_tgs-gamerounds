using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class GetGameRoundByExternalGameGetService : IGetService<GetGameRoundByExternalGameQuery, GameRoundResult?>
    {
        private readonly IGame _game;
        private readonly IGameProvider _gameProvider;
        private readonly IGameRound _gameRoundRepository;

        public GetGameRoundByExternalGameGetService(
            IGame game,
            IGameProvider gameProvider,
            IGameRound gameRoundRepository)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
        }

        public async ValueTask<GameRoundResult?> GetAsync(GetGameRoundByExternalGameQuery query, CancellationToken cancellationToken = default)
        {
            var gameProviderCode = await _gameProvider.GetGameProviderCodeAsync(query.GameProviderId).ConfigureAwait(false);

            var gameId = await _game.GetGameIdAsync(query.GameProviderId, query.ExternalGameId).ConfigureAwait(false);

            var gameRoundInfo = await _gameRoundRepository.GetAsync(query.GameProviderId, query.ExternalRoundId, gameId, query.WalletId).ConfigureAwait(false);

            return gameRoundInfo != null ? new GameRoundResult(
                gameRoundInfo.RoundId,
                gameRoundInfo.ExternalRoundId,
                gameRoundInfo.GameProviderId,
                gameProviderCode,
                gameRoundInfo.PlayerCode,
                gameRoundInfo.GameId,
                query.ExternalGameId,
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
