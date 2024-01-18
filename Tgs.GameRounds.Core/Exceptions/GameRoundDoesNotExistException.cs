namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundDoesNotExistException(Guid roundId) : Exception($"This GameRound does not exist.({roundId})")
{
}
