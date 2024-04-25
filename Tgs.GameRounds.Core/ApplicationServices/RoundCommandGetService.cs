using Microsoft.Extensions.Logging;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class RoundCommandGetService(
	ILogger<RoundCommandGetService> logger,
	ISerialNumberGenerator serialNumberGenerator,
	IRoundRepository repository) : IGetService<RoundCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(RoundCommand query, CancellationToken cancellationToken = default)
	{
		bool isOperate = false;

		var externalClosedOn = (DateTimeOffset?)null;
		var bet = query.Bet;
		var validBet = query.ValidBet;
		var settle = 0.0m;
		var commission = 0.0m;
		var turnover = 0.0m;
		var playerWinLoss = 0.0m;

		if (query.Status == Common.Enums.RoundStatus.Closed)
		{
			externalClosedOn = query.ExternalClosedOn;
			settle = query.Settle;
			commission = query.Commission;
			turnover = query.Turnover;
			playerWinLoss = query.PlayerWinLoss;
		}

		var id = await repository.GetIdAsync(
			gameProviderId: query.GameProviderId,
			externalRoundId: query.ExternalRoundId,
			gameId: query.GameId,
			walletId: query.WalletId,
			externalCreatedOn: query.ExternalCreatedOn,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!id.HasValue)
		{
			id = await serialNumberGenerator.GenerateAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			await repository.AddAsync(
				source: new Models.RoundAdd(
						Id: id.Value,
						GameProviderId: query.GameProviderId,
						ExternalRoundId: query.ExternalRoundId,
						GameId: query.GameId,
						PlayerCode: query.PlayerCode,
						WalletId: query.WalletId,
						CurrencyCode: query.CurrencyCode,
						BrandId: query.BrandId,
						GameProviderConfigurationId: query.GameProviderConfigurationId,
						Status: query.Status,
						ClientStatus: query.ClientStatus,
						ExternalCreatedOn: query.ExternalCreatedOn,
						ExternalClosedOn: externalClosedOn,
						Bet: bet,
						ValidBet: validBet,
						Settle: settle,
						Commission: commission,
						Turnover: turnover,
						PlayerWinLoss: playerWinLoss),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			isOperate = true;
		}
		else
		{
			logger.LogInformation("LogOn:{logOn} Message:This game round information.{info}",
				DateTime.Now.ToString("o"),
				new
				{
					Id = id.Value,
					query.GameProviderId,
					query.GameId,
					query.PlayerCode,
					query.WalletId,
					query.ExternalRoundId,
				});

			var roundInfo = await repository.GetAsync(
				id: id.Value,
				externalCreatedOn: query.ExternalCreatedOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? throw new ArgumentNullException(id.ToString());

			if (roundInfo.Status == Common.Enums.RoundStatus.Open && query.Status == Common.Enums.RoundStatus.Open)
			{
				if (roundInfo.Bet != query.Bet ||
					roundInfo.Bet != query.ValidBet)
				{
					await repository.OpenAsync(
						id: id!.Value,
						source: new Models.RoundOpen(
							Bet: query.Bet,
							ValidBet: query.ValidBet),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
					isOperate = true;
				}
			}
			else
			{
				if (roundInfo.Status == Common.Enums.RoundStatus.Open && query.Status == Common.Enums.RoundStatus.Closed)
				{
					await repository.ClosedAsync(
						id: id!.Value,
						source: new Models.RoundClosed(
							ClientStatus: query.ClientStatus,
							ExternalClosedOn: externalClosedOn!.Value,
							Bet: bet,
							ValidBet: validBet,
							Settle: settle,
							Commission: commission,
							Turnover: turnover,
							PlayerWinLoss: playerWinLoss),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
					isOperate = true;
				}
				else
				{
					if (roundInfo.Status == Common.Enums.RoundStatus.Closed && query.Status == Common.Enums.RoundStatus.Closed)
					{
						if (roundInfo.Bet != bet ||
						roundInfo.ValidBet != validBet ||
						roundInfo.Settle != settle ||
						roundInfo.Commission != commission ||
						roundInfo.Turnover != turnover ||
						roundInfo.PlayerWinLoss != playerWinLoss ||
						roundInfo.ClientStatus != query.ClientStatus)
						{
							await repository.ReopenAsync(
								id!.Value,
								source: new Models.RoundReopen(
									ClientStatus: query.ClientStatus,
									ExternalClosedOn: externalClosedOn!.Value,
									Bet: bet,
									ValidBet: validBet,
									Settle: settle,
									Commission: commission,
									Turnover: turnover,
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
				additionalData: query.AdditionalData.ToJsonString()!,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return id!.Value;
	}
}
