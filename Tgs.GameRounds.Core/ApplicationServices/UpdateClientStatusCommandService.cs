using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateClientStatusCommandService : ICommandService<UpdateClientStatusCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundRecordMessage _gameRoundRecordMessage;
        private readonly ILogger<UpdateClientStatusCommandService> _logger;

        public UpdateClientStatusCommandService(
            ILogger<UpdateClientStatusCommandService> logger,
            IGameRound gameRound,
            IGameRoundRecordMessage gameRoundRecordMessage)
        {
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _gameRoundRecordMessage = gameRoundRecordMessage ?? throw new ArgumentNullException(nameof(gameRoundRecordMessage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(UpdateClientStatusCommand command, CancellationToken cancellationToken = default)
        {
            if (!await _gameRound.ExistAsync(
                roundId: command.RoundId,
                cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundDoesNotExistException", DateTime.Now.ToString("o"), command);
                throw new GameRoundDoesNotExistException();
            }

            await _gameRound.UpdateClientStatusAsync(
                roundId: command.RoundId,
                status: command.Status,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await _gameRoundRecordMessage.SenderGameRoundRecord(
                roundId: command.RoundId,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
