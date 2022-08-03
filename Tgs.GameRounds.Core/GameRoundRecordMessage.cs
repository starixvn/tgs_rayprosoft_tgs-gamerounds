using System.Text;
using System.Text.Json;
using Tgs.MessageQueue;

namespace Tgs.GameRounds.Core
{
    internal class GameRoundRecordMessage : IGameRoundRecordMessage
    {
        private readonly IMessageSender _messageSender;

        public GameRoundRecordMessage(IMessageSender messageSender)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public ValueTask SenderGameRoundRecord(Guid roundId, CancellationToken cancellationToken = default)
            => _messageSender.SendAsync(
                subject: "gp.gameround.gameroundrecord.add",
                data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { RoundId = roundId })),
                cancellationToken: cancellationToken);
    }
}
