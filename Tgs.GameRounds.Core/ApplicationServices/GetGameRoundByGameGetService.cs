using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class GetGameRoundByGameGetService(
    IGameRepository gameRepository,
    IGameProviderRepository gameProviderRepository,
    IGameRoundRepository gameRoundRepository) : IGetService<GetGameRoundByGameQuery, GameRoundResult?>
{
    public async ValueTask<GameRoundResult?> GetAsync(GetGameRoundByGameQuery query, CancellationToken cancellationToken = default)
    {
        var gameRoundInfo = await gameRoundRepository.GetAsync(
            gameProviderId: query.GameProviderId,
            externalRoundId: query.ExternalRoundId,
            gameId: query.GameId,
            walletId: query.WalletId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (gameRoundInfo is not null)
        {
            var gameProviderCode = await gameProviderRepository.GetCode(
                id: gameRoundInfo.GameProviderId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var externalGameId = await gameRepository.GetExternalIdAsync(
                id: gameRoundInfo.GameId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var detailInfo = await gameRoundRepository
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

        return default;
    }
}
