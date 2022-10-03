using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.HandleServices;

internal class GameRoundWorkHandleService : ICommandService<GameRoundWorkCommand>
{
	private readonly ILogger<GameRoundWorkHandleService> _logger;
	private readonly IGameRound _gameRound;
	private readonly ICommandService<AddRoundCommand> _addRoundService;
	private readonly ICommandService<UpdateRoundOpenCommand> _updateRoundOpenService;
	private readonly ICommandService<UpdateRoundClosedCommand> _updateRoundClosedService;
	private readonly ICommandService<UpdateRoundReopenCommand> _updateRoundReopenService;

	public GameRoundWorkHandleService(
		ILogger<GameRoundWorkHandleService> logger,
		IGameRound gameRound,
		ICommandService<AddRoundCommand> addRoundService,
		ICommandService<UpdateRoundOpenCommand> updateRoundOpenService,
		ICommandService<UpdateRoundClosedCommand> updateRoundClosedService,
		ICommandService<UpdateRoundReopenCommand> updateRoundReopenService)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
		_addRoundService = addRoundService ?? throw new ArgumentNullException(nameof(addRoundService));
		_updateRoundOpenService = updateRoundOpenService ?? throw new ArgumentNullException(nameof(updateRoundOpenService));
		_updateRoundClosedService = updateRoundClosedService ?? throw new ArgumentNullException(nameof(updateRoundClosedService));
		_updateRoundReopenService = updateRoundReopenService ?? throw new ArgumentNullException(nameof(updateRoundReopenService));
	}

	public async ValueTask ExecuteAsync(GameRoundWorkCommand command, CancellationToken cancellationToken = default)
	{
		var roundId = await _gameRound.GetRoundIdAsync(
			gameProviderId: command.GameProviderId,
			externalRoundId: command.ExternalRoundId,
			gameId: command.GameId,
			walletId: command.WalletId,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		if (!roundId.HasValue)
		{
			var addRound = new AddRoundCommand(
				ExternalRoundId: command.ExternalRoundId,
				GameProviderId: command.GameProviderId,
				PlayerCode: command.PlayerCode,
				GameId: command.GameId,
				CurrencyCode: command.CurrencyCode,
				Status: command.Status,
				GameProviderConfigurationId: command.GameProviderConfigurationId,
				WalletId: command.WalletId,
				BrandId: command.BrandId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalClosedOn: command.ExternalClosedOn,
				BetAmount: command.BetAmount,
				ValidBetAmount: command.ValidBetAmount,
				SettleAmount: command.SettleAmount,
				Commission: command.Commission,
				Turnover: command.Turnover,
				PlayerWinLoss: command.PlayerWinLoss,
				ClientStatus: command.ClientStatus,
				AdditionalData: command.AdditionalData);

			await _addRoundService
				   .ExecuteAsync(
					   command: addRound,
					   cancellationToken: cancellationToken)
				   .ConfigureAwait(false);
		}
		else
		{
			var gameRound = await _gameRound
					.GetAsync(
						roundId: roundId.Value,
						cancellationToken: cancellationToken)
					.ConfigureAwait(false);

			if (gameRound == null)
			{
				_logger.LogError("GameRound can't find it using the identification number.({roundId})", roundId);
				return;
			}

			if (gameRound.Status == GameRoundStatus.Open && command.Status == GameRoundStatus.Open)
			{
				if (gameRound.BetAmount.GetValueOrDefault(0) != command.BetAmount ||
					gameRound.ValidBetAmount.GetValueOrDefault(0) != command.ValidBetAmount)
				{
					var updateRoundOpen = new UpdateRoundOpenCommand(
						RoundId: gameRound.RoundId,
						BetAmount: command.BetAmount,
						ValidBetAmount: command.ValidBetAmount,
						Commission: command.Commission,
						Turnover: command.Turnover);

					await _updateRoundOpenService
						.ExecuteAsync(
							command: updateRoundOpen,
							cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
			}
			else
			{
				if (gameRound.Status == GameRoundStatus.Open && command.Status == GameRoundStatus.Closed)
				{
					var updateRoundClosed = new UpdateRoundClosedCommand(
						RoundId: gameRound.RoundId,
						ExternalClosedOn: command.ExternalClosedOn!.Value,
						BetAmount: command.BetAmount,
						ValidBetAmount: command.ValidBetAmount,
						SettleAmount: command.SettleAmount,
						Commission: command.Commission,
						Turnover: command.Turnover,
						PlayerWinLoss: command.PlayerWinLoss,
						ClientStatus: command.ClientStatus,
						AdditionalData: command.AdditionalData);

					await _updateRoundClosedService
						.ExecuteAsync(
							command: updateRoundClosed,
							cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				else
				{
					if (gameRound.Status == GameRoundStatus.Closed && command.Status == GameRoundStatus.Closed)
					{
						if (gameRound.BetAmount.GetValueOrDefault(0) != command.BetAmount ||
						gameRound.ValidBetAmount.GetValueOrDefault(0) != command.ValidBetAmount ||
						gameRound.SettleAmount.GetValueOrDefault(0) != command.SettleAmount ||
						gameRound.Commission.GetValueOrDefault(0) != command.Commission ||
						gameRound.Turnover.GetValueOrDefault(0) != command.Turnover ||
						gameRound.CompanyWinLoss.GetValueOrDefault(0) != -command.PlayerWinLoss ||
						gameRound.ClientStatus != command.ClientStatus)
						{

							var updateRoundReopen = new UpdateRoundReopenCommand(
								RoundId: gameRound.RoundId,
								Status: command.Status,
								ExternalClosedOn: command.ExternalClosedOn,
								BetAmount: command.BetAmount,
								ValidBetAmount: command.ValidBetAmount,
								SettleAmount: command.SettleAmount,
								Commission: command.Commission,
								Turnover: command.Turnover,
								PlayerWinLoss: command.PlayerWinLoss,
								ClientStatus: command.ClientStatus,
								AdditionalData: command.AdditionalData
							);

							await _updateRoundReopenService
								.ExecuteAsync(
									command: updateRoundReopen,
									cancellationToken: cancellationToken)
								.ConfigureAwait(false);
						}
					}
				}
			}
		}
	}
}
