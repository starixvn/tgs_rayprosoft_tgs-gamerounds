using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateRoundOpenCommandService : ICommandService<UpdateRoundOpenCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly ILogger<UpdateRoundOpenCommandService> _logger;

        public UpdateRoundOpenCommandService(
          IGameRoundRecordRepository gameRoundRecordRepository,
          IGameRound gameRound,
           ILogger<UpdateRoundOpenCommandService> logger)
        {
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(UpdateRoundOpenCommand command, CancellationToken cancellationToken = default)
        {
            if (!await _gameRound.ExistAsync(command.RoundId).ConfigureAwait(false))
            {
                _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundDoesNotExistException", DateTime.Now.ToString("o"), command);
                throw new GameRoundDoesNotExistException();
            }

            await _gameRound.UpdateRoundOpenAsync(new UpdateRoundOpen(
                RoundId: command.RoundId,
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                Commission: command.Commission,
                Turnover: command.Turnover)).ConfigureAwait(false);

            var round = await _gameRound.GetAsync(command.RoundId).ConfigureAwait(false);

            if (round is not null)
            {
                await _gameRoundRecordRepository.AddAsync(round).ConfigureAwait(false);
            }
        }
    }
}
