using MediatR;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class RoundGetExternalGameService(
	TimeProvider timeProvider,
	IGameRepository gameRepository,
	IGameProviderRepository gameProviderRepository,
	IRoundRepository gameRoundRepository) : IRequestHandler<RoundGetExternalGameRequest, RoundInfoResponse?>
{
	public async Task<RoundInfoResponse?> Handle(RoundGetExternalGameRequest request, CancellationToken cancellationToken)
	{
		var externalCreatedOn = request.ExternalCreatedOn ?? timeProvider.GetUtcNow().AddMonths(-3);

		var gameProviderCode = await gameProviderRepository.GetCodeAsync(
			id: request.GameProviderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameId = await gameRepository.GetIdAsync(
			gameProviderId: request.GameProviderId,
			externalId: request.ExternalGameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var roundInfo = await gameRoundRepository.GetAsync(
			externalCreatedOn: externalCreatedOn,
			gameProviderId: request.GameProviderId,
			gameId: gameId,
			walletId: request.WalletId,
			externalRoundId: request.ExternalRoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (roundInfo is null)
		{
			return default;
		}

		var detailInfo = await gameRoundRepository.GetRoundDetailAsync(
			id: roundInfo.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new RoundInfoResponse(
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
			ExternalGameId: request.ExternalGameId,
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
