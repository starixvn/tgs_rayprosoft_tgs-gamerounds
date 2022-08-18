using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class UpdateStatusCommandService : ICommandService<UpdateStatusCommand>
{
	private readonly IGameRound _gameRound;
	private readonly IGameRoundRecordMessage _gameRoundRecordMessage;
	private readonly ILogger<UpdateStatusCommandService> _logger;

	public UpdateStatusCommandService(
		ILogger<UpdateStatusCommandService> logger,
		IGameRound gameRound,
		IGameRoundRecordMessage gameRoundRecordMessage)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
		_gameRoundRecordMessage = gameRoundRecordMessage ?? throw new ArgumentNullException(nameof(gameRoundRecordMessage));
	}

	public async ValueTask ExecuteAsync(UpdateStatusCommand command, CancellationToken cancellationToken = default)
	{
		if (!await _gameRound.ExistAsync(
			roundId: command.RoundId,
			cancellationToken: cancellationToken).ConfigureAwait(false))
		{
			_logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundDoesNotExistException", DateTime.Now.ToString("o"), command);
			throw new GameRoundDoesNotExistException();
		}

		await _gameRound.UpdateStatusAsync(
			roundId: command.RoundId,
			status: command.Status,
			externalClosedOn: command.ExternalClosedOn,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		await _gameRoundRecordMessage.SenderGameRoundRecord(
			roundId: command.RoundId,
			cancellationToken: cancellationToken).ConfigureAwait(false);
	}
}
