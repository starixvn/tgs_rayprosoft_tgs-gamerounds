using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class GetGameRoundGetService : IGetService<GetGameRoundQuery, GameRoundResult?>
{
    private readonly IGameRepository _game;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameRoundRepository _gameRoundRepository;

    public GetGameRoundGetService(
        IGameRepository game,
        IGameProviderRepository gameProvider,
        IGameRoundRepository gameRoundRepository)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
    }

    public async ValueTask<GameRoundResult?> GetAsync(GetGameRoundQuery query, CancellationToken cancellationToken = default)
    {
        var gameRoundInfo = await _gameRoundRepository
            .GetAsync(
                query.RoundId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gameRoundInfo is null)
        {
            return default;
        }

        var gameProviderCode = await _gameProvider.GetCode(
            id: gameRoundInfo.GameProviderId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var externalGameId = await _game.GetExternalIdAsync(
            id: gameRoundInfo.GameId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var detailInfo = await _gameRoundRepository
            .GetRoundDetailAsync(
                gameRoundInfo.RoundId,
                cancellationToken)
            .ConfigureAwait(false);

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
            gameRoundInfo.EffectiveBettingPercentage,
            detailInfo?.AdditionalData ?? string.Empty);
    }
}
