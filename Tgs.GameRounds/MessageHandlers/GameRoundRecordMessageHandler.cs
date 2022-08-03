using System.Text;
using System.Text.Json;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.MessageHandlers
{
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
            var model = JsonSerializer.Deserialize<GameRoundRecordMessageModel>(json);

            if (model == null)
            {
                _logger.LogError("LogOn:{logOn} Information Received:{model}", DateTime.Now.ToString("o"), model);
                return ValueTask.CompletedTask;
            }

            _logger.LogInformation("LogOn:{logOn} Information Received:{model}", DateTime.Now.ToString("o"), model);

            var command = new AddGameRoundRecordCommand(
                RoundId: model.RoundId);

            return _service.ExecuteAsync(
                command: command,
                cancellationToken: cancellationToken);
        }
    }
}
