using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class AddRoundBatchCommandService : IQueryService<AddRoundBatchCommand, Guid>
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

    public async IAsyncEnumerable<Guid> QueryAsync(AddRoundBatchCommand command, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var roundIds = await _gameRound
                .AddRoundBatchAsync(
                    sources: command.AddRoundBatches,
                    cancellationToken: cancellationToken)
                .ToListAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        _logger.LogInformation("LogOn:{DateTime} | Command: {command} | Message:Batch added successfully | RoundIds:{roundIds}", DateTime.Now.ToString("o"), command, roundIds);

        if (!roundIds.Any())
        {
            yield break;
        }

        foreach (var roundId in roundIds)
        {
            await _gameRoundRecordMessage
                    .SenderGameRoundRecord(
                        roundId: roundId,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            yield return roundId;
        }
    }
}
