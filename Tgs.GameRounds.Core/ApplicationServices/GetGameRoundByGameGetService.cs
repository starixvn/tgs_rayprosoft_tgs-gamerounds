using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class GetGameRoundByGameGetService : IGetService<GetGameRoundByGameQuery, GameRoundResult?>
{
    private readonly IGameRepository _game;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameRoundRepository _gameRoundRepository;

    public GetGameRoundByGameGetService(
        IGameRepository game,
        IGameProviderRepository gameProvider,
        IGameRoundRepository gameRoundRepository)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
    }

    public async ValueTask<GameRoundResult?> GetAsync(GetGameRoundByGameQuery query, CancellationToken cancellationToken = default)
    {
        var gameRoundInfo = await _gameRoundRepository.GetAsync(
            gameProviderId: query.GameProviderId,
            externalRoundId: query.ExternalRoundId,
            gameId: query.GameId,
            walletId: query.WalletId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (gameRoundInfo is not null)
        {
            var gameProviderCode = await _gameProvider.GetCode(
                id: query.GameProviderId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var externalGameId = await _game.GetExternalIdAsync(
                id: query.GameId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

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
                gameRoundInfo.CompanyWinLoss,
                gameRoundInfo.TieBetAdjustment,
                gameRoundInfo.ClientStatus,
                gameRoundInfo.ReopenTurnoverAdjustment,
                gameRoundInfo.ReopenedOn,
                gameRoundInfo.EffectiveBettingPercentage);
        }

        return default;
    }
}
