using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class RoundByGameGetService(
	TimeProvider timeProvider,
	IGameRepository gameRepository,
	IGameProviderRepository gameProviderRepository,
	IRoundRepository gameRoundRepository) : IGetService<RoundByGameQuery, RoundResult?>
{
	public async ValueTask<RoundResult?> GetAsync(RoundByGameQuery query, CancellationToken cancellationToken = default)
	{
		var externalCreatedOn = query.ExternalCreatedOn ?? timeProvider.GetUtcNow().AddMonths(-3);

		var roundInfo = await gameRoundRepository.GetAsync(
			gameProviderId: query.GameProviderId,
			externalRoundId: query.ExternalRoundId,
			gameId: query.GameId,
			walletId: query.WalletId,
			externalCreatedOn: externalCreatedOn,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		if (roundInfo is not null)
		{
			var gameProviderCode = await gameProviderRepository.GetCode(
				id: roundInfo.GameProviderId,
				cancellationToken: cancellationToken).ConfigureAwait(false);

			var externalGameId = await gameRepository.GetExternalIdAsync(
				id: roundInfo.GameId,
				cancellationToken: cancellationToken).ConfigureAwait(false);

			var detailInfo = await gameRoundRepository
				.GetRoundDetailAsync(
					roundInfo.Id,
					cancellationToken)
				.ConfigureAwait(false);

			return new RoundResult(
				RoundId: roundInfo.Id,
				GameProviderId: roundInfo.GameProviderId,
				GameProviderCode: gameProviderCode,
				GameProviderConfigurationId: roundInfo.GameProviderConfigurationId,
				GameId: roundInfo.GameId,
				BrandId: roundInfo.BrandId,
				PlayerCode: roundInfo.PlayerCode,
				WalletId: roundInfo.WalletId,
				CurrencyCode: roundInfo.CurrencyCode,
				Status: roundInfo.Status,
				ClientStatus: roundInfo.ClientStatus,
				ExternalRoundId: roundInfo.ExternalRoundId,
				ExternalGameId: externalGameId,
				ExternalCreatedOn: roundInfo.ExternalCreatedOn,
				ExternalClosedOn: roundInfo.ExternalClosedOn,
				BetAmount: roundInfo.Bet,
				ValidBetAmount: roundInfo.ValidBet,
				SettleAmount: roundInfo.Settle,
				Commission: roundInfo.Commission,
				Turnover: roundInfo.Turnover,
				PlayerWinLoss: roundInfo.PlayerWinLoss,
				CreatedOn: roundInfo.CreatedOn,
				ClosedOn: roundInfo.ClosedOn,
				UpdatedOn: roundInfo.UpdatedOn,
				AdditionalData: detailInfo?.AdditionalData ?? string.Empty);
		}

		return default;
	}
}
