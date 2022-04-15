using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class GetGameRoundGetService : IGetService<GetGameRoundQuery, GameRoundResult?>
    {
        private readonly IGame _game;
        private readonly IGameProvider _gameProvider;
        private readonly IGameRound _gameRoundRepository;

        public GetGameRoundGetService(
            IGame game,
            IGameProvider gameProvider,
            IGameRound gameRoundRepository)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
        }

        public async ValueTask<GameRoundResult?> GetAsync(GetGameRoundQuery query, CancellationToken cancellationToken = default)
        {
            var gameRoundInfo = await _gameRoundRepository.GetAsync(query.RoundId).ConfigureAwait(false);

            if (gameRoundInfo is not null)
            {
                var gameProviderCode = await _gameProvider.GetGameProviderCodeAsync(gameRoundInfo.GameProviderId).ConfigureAwait(false);

                var externalGameId = await _game.GetExternalGameIdAsync(gameRoundInfo.GameId).ConfigureAwait(false);

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

            return default;
        }
    }
}
