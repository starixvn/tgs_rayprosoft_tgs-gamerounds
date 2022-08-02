using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateClientStatusCommandService : ICommandService<UpdateClientStatusCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly ILogger<UpdateClientStatusCommandService> _logger;

        public UpdateClientStatusCommandService(
          IGameRoundRecordRepository gameRoundRecordRepository,
          IGameRound gameRound,
          ILogger<UpdateClientStatusCommandService> logger)
        {
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
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
