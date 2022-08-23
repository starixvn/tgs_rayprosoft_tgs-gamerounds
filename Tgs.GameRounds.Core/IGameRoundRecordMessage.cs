namespace Tgs.GameRounds.Core;

public interface IGameRoundRecordMessage
{
	ValueTask SenderGameRoundRecord(Guid roundId, CancellationToken cancellationToken = default);
}
