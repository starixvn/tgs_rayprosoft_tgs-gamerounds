using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class GameRoundByExternalGameGetService(
    IGameRepository gameRepository,
    IGameProviderRepository gameProviderRepository,
    IGameRoundRepository gameRoundRepository) : IGetService<GameRoundByExternalGameQuery, GameRoundResult?>
{
    public async ValueTask<GameRoundResult?> GetAsync(GameRoundByExternalGameQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderCode = await gameProviderRepository.GetCode(
            id: query.GameProviderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameId = await gameRepository.GetIdAsync(
            gameProviderId: query.GameProviderId,
            externalId: query.ExternalGameId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameRoundInfo = await gameRoundRepository.GetAsync(
            gameProviderId: query.GameProviderId,
            externalRoundId: query.ExternalRoundId,
            gameId: gameId,
            walletId: query.WalletId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gameRoundInfo is null)
        {
            return null;
        }

        var detailInfo = await gameRoundRepository.GetRoundDetailAsync(
            id: gameRoundInfo.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new GameRoundResult(
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
            gameRoundInfo.EffectiveBettingPercentage,
            detailInfo?.AdditionalData ?? string.Empty);
    }
}
