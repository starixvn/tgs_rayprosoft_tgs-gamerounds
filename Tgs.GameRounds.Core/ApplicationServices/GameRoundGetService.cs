using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class GameRoundGetService(
    IGameRepository gameRepository,
    IGameProviderRepository gameProviderRepository,
    IGameRoundRepository gameRoundRepository) : IGetService<GameRoundQuery, GameRoundResult?>
{
    public async ValueTask<GameRoundResult?> GetAsync(GameRoundQuery query, CancellationToken cancellationToken = default)
    {
        var gameRoundInfo = await gameRoundRepository.GetAsync(
            query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gameRoundInfo is null)
        {
            return default;
        }

        var gameProviderCode = await gameProviderRepository.GetCode(
            id: gameRoundInfo.GameProviderId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var externalGameId = await gameRepository.GetExternalIdAsync(
            id: gameRoundInfo.GameId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var detailInfo = await gameRoundRepository.GetRoundDetailAsync(
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
