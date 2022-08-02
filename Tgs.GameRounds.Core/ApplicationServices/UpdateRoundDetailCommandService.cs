using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateRoundDetailCommandService : ICommandService<UpdateRoundDetailCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly ILogger<UpdateRoundDetailCommandService> _logger;

        public UpdateRoundDetailCommandService(
          IGameRound gameRound,
          ILogger<UpdateRoundDetailCommandService> logger)
        {
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(UpdateRoundDetailCommand command, CancellationToken cancellationToken = default)
        {
            if (!await _gameRound.ExistAsync(
                roundId: command.RoundId,
                cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundDoesNotExistException", DateTime.Now.ToString("o"), command);
                throw new GameRoundDoesNotExistException();
            }

            await _gameRound.UpdateRoundDetailAsync(
                roundId: command.RoundId,
                additionalData: command.AdditionalData,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
