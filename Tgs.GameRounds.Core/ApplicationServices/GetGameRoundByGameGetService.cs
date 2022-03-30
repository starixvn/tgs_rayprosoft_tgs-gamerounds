using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class GetGameRoundByGameGetService : IGetService<GetGameRoundByGameQuery, GameRoundResult?>
    {
        private readonly IGame _game;
        private readonly IGameProvider _gameProvider;
        private readonly IGameRoundRepository _gameRoundRepository;

        public GetGameRoundByGameGetService(
            IGame game,
            IGameProvider gameProvider,
            IGameRoundRepository gameRoundRepository)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
        }

        public async ValueTask<GameRoundResult?> GetAsync(GetGameRoundByGameQuery query, CancellationToken cancellationToken = default)
        {
            var gameRoundInfo = await _gameRoundRepository.GetAsync(query.GameProviderId, query.ExternalRoundId, query.GameId, query.WalletId).ConfigureAwait(false);

            if (gameRoundInfo == null)
            {
                return null;
            }

            var gameProviderCode = await _gameProvider.GetGameProviderCodeAsync(query.GameProviderId).ConfigureAwait(false);

            var externalGameId = await _game.GetExternalGameIdAsync(query.GameId).ConfigureAwait(false);

            return new GameRoundResult(
                gameRoundInfo.RoundId,
                gameRoundInfo.ExternalRoundId,
                gameRoundInfo.GameProviderId,
                gameProviderCode,
                gameRoundInfo.PlayerCode,
                gameRoundInfo.GameId,
                externalGameId,
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
                gameRoundInfo.AdditionalData);
        }
    }
}
