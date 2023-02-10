using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Common.Protos;
using Tgs.MessageQueue;

namespace Tgs.GameRounds.Core;

internal class GameRoundRecordMessage : IGameRoundRecordMessage
{
    private readonly ILogger<GameRoundRecordMessage> _logger;
    private readonly IMessageSender _messageSender;

    public GameRoundRecordMessage(
        ILogger<GameRoundRecordMessage> logger,
        IMessageSender messageSender)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
    }

    public async ValueTask SenderGameRoundRecord(Guid roundId, CancellationToken cancellationToken = default)
    {
        var request = new GameRoundRecordRequest
        {
            Id = roundId,
            SendOn = DateTimeOffset.UtcNow.ToTimestamp(),
        };

        try
        {
            await _messageSender.PublishAsync(
                subject: "game.gameround.gameroundrecord.add",
                data: request.ToByteArray(),
                cancellationToken: cancellationToken);

            _logger.LogInformation("LogOn:{logOn} Information Sending:{request}", DateTime.Now.ToString("o"), request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} Information Sending:{request}", DateTime.Now.ToString("o"), request);
        }
    }
}
