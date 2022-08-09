using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class AddRoundBatchCommandService : IGetService<AddRoundBatchCommand, IEnumerable<Guid>>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundRecordMessage _gameRoundRecordMessage;
        private readonly ILogger<AddRoundBatchCommandService> _logger;

        public AddRoundBatchCommandService(
            ILogger<AddRoundBatchCommandService> logger,
            IGameRound gameRound,
            IGameRoundRecordMessage gameRoundRecordMessage)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _gameRoundRecordMessage = gameRoundRecordMessage ?? throw new ArgumentNullException(nameof(gameRoundRecordMessage));
        }

        public async ValueTask<IEnumerable<Guid>> GetAsync(AddRoundBatchCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var roundIds = await _gameRound.AddRoundBatchAsync(
                    sources: command.AddRoundBatches,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (roundIds.Any())
                {
                    foreach (var roundId in roundIds)
                    {
                        await _gameRoundRecordMessage
                                .SenderGameRoundRecord(
                                    roundId: roundId,
                                    cancellationToken: cancellationToken)
                                .ConfigureAwait(false);
                    }
                }
                _logger.LogInformation("LogOn:{DateTime} | Command: {command} | Message:Batch added successfully | RoundIds:{roundIds}", DateTime.Now.ToString("o"), command, roundIds);

                return roundIds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{DateTime} | Command: {command} | Message:Error in batch addition", DateTime.Now.ToString("o"), command);

                return Enumerable.Empty<Guid>();
            }
        }
    }
}
