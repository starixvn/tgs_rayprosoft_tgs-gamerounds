using System.Text;
using System.Text.Json;
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
		var json = Encoding.UTF8.GetString(data.Span);
		var receive = JsonSerializer.Deserialize<GameRoundRecordReceive>(json);

		if (receive == null)
		{
			_logger.LogError("LogOn:{logOn} Information Received:{receive}", DateTime.Now.ToString("o"), receive);
			return ValueTask.CompletedTask;
		}

		_logger.LogInformation("LogOn:{logOn} Information Received:{receive}", DateTime.Now.ToString("o"), receive);

		var command = new AddGameRoundRecordCommand(
			RoundId: receive.RoundId,
			SendOn: receive.SendOn,
			ReceiveOn: DateTimeOffset.UtcNow);

		return _service.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
