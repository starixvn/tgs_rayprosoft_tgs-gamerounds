using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class AddRoundCommandService : ICommandService<AddRoundCommand>
{
	private readonly IGameRound _gameRound;
	private readonly IGameRoundLocked _gameRoundLocked;
	private readonly IGameRoundRecordMessage _gameRoundRecordMessage;
	private readonly ILogger<AddRoundCommandService> _logger;

	public AddRoundCommandService(
		ILogger<AddRoundCommandService> logger,
		IGameRound gameRound,
		IGameRoundLocked gameRoundLocked,
		IGameRoundRecordMessage gameRoundRecordMessage)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
		_gameRoundLocked = gameRoundLocked ?? throw new ArgumentNullException(nameof(gameRoundLocked));
		_gameRoundRecordMessage = gameRoundRecordMessage ?? throw new ArgumentNullException(nameof(gameRoundRecordMessage));
	}

	public async ValueTask ExecuteAsync(AddRoundCommand command, CancellationToken cancellationToken = default)
	{
		var roundId = await _gameRound.GetRoundIdAsync(
				gameProviderId: command.GameProviderId,
				externalRoundId: command.ExternalRoundId,
				gameId: command.GameId,
				walletId: command.WalletId,
				cancellationToken: cancellationToken).ConfigureAwait(false);

		if (roundId.HasValue)
		{
			return;
		}

		if (!await _gameRoundLocked.LockingAsync(
				gameProviderId: command.GameProviderId,
				externalRoundId: command.ExternalRoundId,
				gameId: command.GameId,
				walletId: command.WalletId,
				cancellationToken: cancellationToken).ConfigureAwait(false))
		{
			throw new GameRoundCreationFailedException();
		}

		try
		{
			var addRound = new AddRound(
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

			var newRoundId = await _gameRound.AddRoundAsync(
				source: addRound,
				cancellationToken: cancellationToken).ConfigureAwait(false);

			if (!newRoundId.HasValue)
			{
				_logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundCreationFailedException", DateTime.Now.ToString("o"), command);
				throw new GameRoundCreationFailedException();
			}
			else
			{
				await _gameRoundRecordMessage.SenderGameRoundRecord(
					roundId: newRoundId.Value,
					cancellationToken: cancellationToken).ConfigureAwait(false);
			}
		}
		finally
		{
			await _gameRoundLocked.UnlockAsync(
					gameProviderId: command.GameProviderId,
					externalRoundId: command.ExternalRoundId,
					gameId: command.GameId,
					walletId: command.WalletId,
					cancellationToken: cancellationToken).ConfigureAwait(false);
		}
	}
}
