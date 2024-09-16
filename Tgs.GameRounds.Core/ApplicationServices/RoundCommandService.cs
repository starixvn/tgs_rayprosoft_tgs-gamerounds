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
		bool isOperate = false;

		var externalClosedOn = (DateTimeOffset?)null;
		var bet = request.Bet;
		var validBet = request.ValidBet;
		var settle = 0.0m;
		var commission = 0.0m;
		var playerWinLoss = 0.0m;

		if (request.Status == Common.Enums.RoundStatus.Closed)
		{
			externalClosedOn = request.ExternalClosedOn;
			settle = request.Settle;
			commission = request.Commission;
			validBet = request.ValidBet;
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
					PlayerWinLoss: playerWinLoss,
					CreatedOn: request.CreatedOn),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			isOperate = true;
		}
		else
		{
			logger.LogWarning("{logWarning}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					request.Id,
					request.GameProviderId,
					request.GameId,
					request.PlayerCode,
					request.WalletId,
					request.ExternalRoundId,
					ExternalCreatedOn = request.ExternalCreatedOn.ToString("u"),
					RequestStatus = request.Status.ToString(),
					RequestClientStatus = request.ClientStatus.ToString(),
					Message = "This game round information."
				}));

			var roundInfo = await repository.GetAsync(
				id: id.Value,
				externalCreatedOn: request.ExternalCreatedOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? throw new ArgumentNullException(id.ToString());

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
							ValidBet: validBet),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
					isOperate = true;
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
							PlayerWinLoss: playerWinLoss),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
					isOperate = true;
				}
				else
				{
					if (roundInfo.Status == Common.Enums.RoundStatus.Closed && request.Status == Common.Enums.RoundStatus.Closed)
					{
						if (roundInfo.Bet != bet ||
						roundInfo.ValidBet != validBet ||
						roundInfo.Settle != settle ||
						roundInfo.Commission != commission ||
						roundInfo.PlayerWinLoss != playerWinLoss ||
						roundInfo.ClientStatus != request.ClientStatus)
						{
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
									PlayerWinLoss: playerWinLoss),
								cancellationToken: cancellationToken)
								.ConfigureAwait(false);
							isOperate = true;
						}
					}
				}
			}
		}

		if (isOperate)
		{
			await repository.MergeDetailAsync(
				id: id.Value,
				additionalData: request.AdditionalData.ToJsonString()!,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return id!.Value;
	}
}
