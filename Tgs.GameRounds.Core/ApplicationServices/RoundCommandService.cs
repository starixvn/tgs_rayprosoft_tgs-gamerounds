using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class RoundCommandService(
	ILogger<RoundCommandService> logger,
	IRoundRepository repository) : IRequestHandler<RoundCommandRequest, Guid>
{
	public async Task<Guid> Handle(RoundCommandRequest request, CancellationToken cancellationToken)
	{
		var externalClosedOn = (DateTimeOffset?)null;
		var bet = request.Bet;
		var validBet = request.ValidBet;
		var turnover = 0.0m;
		var settle = 0.0m;
		var commission = 0.0m;
		var playerWinLoss = 0.0m;

		if (request.Status == Common.Enums.RoundStatus.Closed)
		{
			externalClosedOn = request.ExternalClosedOn;
			settle = request.Settle;
			commission = request.Commission;
			validBet = request.ValidBet;
			turnover = request.Turnover;
			playerWinLoss = request.PlayerWinLoss;
		}

		var id = await repository.GetIdAsync(
			gameProviderId: request.GameProviderId,
			externalRoundId: request.ExternalRoundId,
			gameId: request.GameId,
			walletId: request.WalletId,
			externalCreatedOn: request.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!id.HasValue)
		{
			id = request.Id;
			await repository.AddAsync(
				source: new Models.RoundAdd(
					Id: id.Value,
					GameProviderId: request.GameProviderId,
					ExternalRoundId: request.ExternalRoundId,
					GameId: request.GameId,
					PlayerCode: request.PlayerCode,
					WalletId: request.WalletId,
					CurrencyCode: request.CurrencyCode,
					BrandId: request.BrandId,
					GameProviderConfigurationId: request.GameProviderConfigurationId,
					Status: request.Status,
					ClientStatus: request.ClientStatus,
					ExternalCreatedOn: request.ExternalCreatedOn,
					ExternalClosedOn: externalClosedOn,
					Bet: bet,
					ValidBet: validBet,
					Settle: settle,
					Commission: commission,
					Turnover: turnover,
					PlayerWinLoss: playerWinLoss,
					CreatedOn: request.CreatedOn),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		else
		{
			var roundInfo = await repository.GetAsync(
				id: id.Value,
				externalCreatedOn: request.ExternalCreatedOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? throw new ArgumentNullException(id.ToString());

			logger.LogInformation("{logInformation}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					roundInfo.Id,
					roundInfo.GameProviderId,
					roundInfo.GameId,
					roundInfo.PlayerCode,
					roundInfo.WalletId,
					roundInfo.ExternalRoundId,
					ExternalCreatedOn = roundInfo.ExternalCreatedOn!.Value.ToString("u"),
					CurrentStatus = roundInfo.Status.ToString(),
					CurrentClientStatus = roundInfo.Status.ToString(),
					RequestStatus = request.Status.ToString(),
					RequestClientStatus = request.ClientStatus.ToString(),
					Message = "This game round information."
				}));

			if ((roundInfo.Status == Common.Enums.RoundStatus.New || roundInfo.Status == Common.Enums.RoundStatus.Open) &&
				(request.Status == Common.Enums.RoundStatus.New || request.Status == Common.Enums.RoundStatus.Open))
			{
				if (roundInfo.Bet != bet ||
					roundInfo.ValidBet != validBet)
				{
					await repository.OpenAsync(
						externalCreatedOn: request.ExternalCreatedOn,
						id: id!.Value,
						source: new Models.RoundOpen(
							Bet: bet,
							ValidBet: validBet,
							Turnover: turnover),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
			}
			else
			{
				if ((roundInfo.Status == Common.Enums.RoundStatus.New || roundInfo.Status == Common.Enums.RoundStatus.Open) &&
					request.Status == Common.Enums.RoundStatus.Closed)
				{
					await repository.ClosedAsync(
						externalCreatedOn: request.ExternalCreatedOn,
						id: id!.Value,
						source: new Models.RoundClosed(
							ClientStatus: request.ClientStatus,
							ExternalClosedOn: externalClosedOn!.Value,
							Bet: bet,
							ValidBet: validBet,
							Settle: settle,
							Commission: commission,
							Turnover: turnover,
							PlayerWinLoss: playerWinLoss),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				else
				{
					if (roundInfo.Status == Common.Enums.RoundStatus.Closed && request.Status == Common.Enums.RoundStatus.Closed)
					{
						if (roundInfo.Bet != bet ||
						roundInfo.ValidBet != validBet ||
						roundInfo.Settle != settle ||
						roundInfo.Commission != commission ||
						roundInfo.Turnover != turnover ||
						roundInfo.PlayerWinLoss != playerWinLoss ||
						roundInfo.ClientStatus != request.ClientStatus)
						{
							logger.LogWarning("{logWarning}",
								JsonSerializer.Serialize(new
								{
									LogOn = DateTime.Now.ToString("u"),
									roundInfo.Id,
									roundInfo.GameProviderId,
									roundInfo.GameId,
									roundInfo.PlayerCode,
									roundInfo.WalletId,
									roundInfo.ExternalRoundId,
									ExternalCreatedOn = roundInfo.ExternalCreatedOn!.Value.ToString("u"),
									CurrentStatus = roundInfo.Status.ToString(),
									CurrentClientStatus = roundInfo.Status.ToString(),
									CurrentBet = roundInfo.Bet,
									CurrentValidBet = roundInfo.ValidBet,
									CurrentSettle = roundInfo.Settle,
									CurrentCommission = roundInfo.Commission,
									CurrentTurnover = roundInfo.Turnover,
									CurrentPlayerWinLoss = roundInfo.PlayerWinLoss,
									RequestStatus = request.Status.ToString(),
									RequestClientStatus = request.ClientStatus.ToString(),
									RequestBet = bet,
									RequestValidBet = validBet,
									RequestSettle = settle,
									RequestCommission = commission,
									RequestTurnover = turnover,
									RequestPlayerWinLoss = playerWinLoss,
									Message = "This game round data has changed."
								}));

							await repository.ReopenAsync(
								externalCreatedOn: request.ExternalCreatedOn,
								id!.Value,
								source: new Models.RoundReopen(
									ClientStatus: request.ClientStatus,
									ExternalClosedOn: externalClosedOn!.Value,
									Bet: bet,
									ValidBet: validBet,
									Settle: settle,
									Commission: commission,
									Turnover: turnover,
									PlayerWinLoss: playerWinLoss),
								cancellationToken: cancellationToken)
								.ConfigureAwait(false);
						}
					}
				}
			}
		}

		return id!.Value;
	}
}
