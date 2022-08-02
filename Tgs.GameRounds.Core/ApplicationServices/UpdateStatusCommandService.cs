using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateStatusCommandService : ICommandService<UpdateStatusCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly ILogger<UpdateStatusCommandService> _logger;

        public UpdateStatusCommandService(
            IGameRoundRecordRepository gameRoundRecordRepository,
            IGameRound gameRound,
            ILogger<UpdateStatusCommandService> logger)
        {
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            var round = await _gameRound.GetAsync(
                roundId: command.RoundId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (round is not null)
            {
                await _gameRoundRecordRepository.AddAsync(
                    gameRound: round,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
