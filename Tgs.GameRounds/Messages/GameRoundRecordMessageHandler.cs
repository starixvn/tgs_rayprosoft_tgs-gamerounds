using Tgs.GameRounds.Common.Protos;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.MessageHandlers;

public class GameRoundRecordMessageHandler : IMessageHandler
{
    private readonly ILogger<GameRoundRecordMessageHandler> _logger;
    private readonly ICommandService<AddGameRoundRecordCommand> _service;

    public GameRoundRecordMessageHandler(
        ILogger<GameRoundRecordMessageHandler> logger,
        ICommandService<AddGameRoundRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    ValueTask IMessageHandler.HandleAsync(
        ReadOnlyMemory<byte> data,
        CancellationToken cancellationToken)
    {
        var request = GameRoundRecordRequest.Parser.ParseFrom(data.Span) ??
            throw new NullReferenceException("The request data resolves to a null value");

        _logger.LogInformation("LogOn:{logOn} Information Received:{receive}", DateTime.Now.ToString("o"), request);

        var command = new AddGameRoundRecordCommand(
            RoundId: request.Id.ToGuid(),
            SendOn: request.SendOn.ToDateTimeOffset(),
            ReceiveOn: DateTimeOffset.UtcNow);

        return _service.ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
