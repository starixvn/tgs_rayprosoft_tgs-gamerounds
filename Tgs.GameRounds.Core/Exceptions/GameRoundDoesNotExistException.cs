namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundDoesNotExistException : Exception
{
    public GameRoundDoesNotExistException(Guid roundId) : base($"This GameRound does not exist.({roundId})")
    {
    }
}
