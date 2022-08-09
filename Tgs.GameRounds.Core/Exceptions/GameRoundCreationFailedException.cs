namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundCreationFailedException : Exception
{
    public GameRoundCreationFailedException() : base("This GameRound failed to create an order.")
    {
    }

    public GameRoundCreationFailedException(string message) : base(message)
    {
    }
}
