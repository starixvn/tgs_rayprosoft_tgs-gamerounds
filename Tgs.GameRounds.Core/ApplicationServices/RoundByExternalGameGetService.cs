using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class RoundByExternalGameGetService(
	TimeProvider timeProvider,
	IGameRepository gameRepository,
	IGameProviderRepository gameProviderRepository,
	IRoundRepository gameRoundRepository) : IGetService<RoundByExternalGameQuery, RoundResult?>
{
	public async ValueTask<RoundResult?> GetAsync(RoundByExternalGameQuery query, CancellationToken cancellationToken = default)
	{
		var externalCreatedOn = query.ExternalCreatedOn ?? timeProvider.GetUtcNow().AddMonths(-3);

		var gameProviderCode = await gameProviderRepository.GetCode(
			id: query.GameProviderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameId = await gameRepository.GetIdAsync(
			gameProviderId: query.GameProviderId,
			externalId: query.ExternalGameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var roundInfo = await gameRoundRepository.GetAsync(
			gameProviderId: query.GameProviderId,
			gameId: gameId,
			walletId: query.WalletId,
			externalRoundId: query.ExternalRoundId,
			externalCreatedOn: externalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (roundInfo is null)
		{
			return null;
		}

		var detailInfo = await gameRoundRepository.GetRoundDetailAsync(
			id: roundInfo.Id,
			cancellationToken: cancellationToken)
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
			ExternalGameId: query.ExternalGameId,
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
}
