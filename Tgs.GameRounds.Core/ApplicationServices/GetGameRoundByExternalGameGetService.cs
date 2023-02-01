using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class GetGameRoundByExternalGameGetService : IGetService<GetGameRoundByExternalGameQuery, GameRoundResult?>
{
    private readonly IGameRepository _game;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameRoundRepository _gameRoundRepository;

    public GetGameRoundByExternalGameGetService(
        IGameRepository game,
        IGameProviderRepository gameProvider,
        IGameRoundRepository gameRoundRepository)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
    }

    public async ValueTask<GameRoundResult?> GetAsync(GetGameRoundByExternalGameQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderCode = await _gameProvider.GetCode(
            id: query.GameProviderId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var gameId = await _game.GetIdAsync(
            gameProviderId: query.GameProviderId,
            externalId: query.ExternalGameId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var gameRoundInfo = await _gameRoundRepository.GetAsync(
            gameProviderId: query.GameProviderId,
            externalRoundId: query.ExternalRoundId,
            gameId: gameId,
            walletId: query.WalletId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

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
            gameRoundInfo.CompanyWinLoss,
            gameRoundInfo.TieBetAdjustment,
            gameRoundInfo.ClientStatus,
            gameRoundInfo.ReopenTurnoverAdjustment,
            gameRoundInfo.ReopenedOn,
            gameRoundInfo.EffectiveBettingPercentage) : null;
    }
}
