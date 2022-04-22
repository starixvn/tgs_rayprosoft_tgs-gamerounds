namespace Tgs.GameRounds.Core.Exceptions
{
    public class GameRoundDoesNotExistException : Exception
    {
        public GameRoundDoesNotExistException() : base("This GameRound does not exist.")
        {
        }

        public GameRoundDoesNotExistException(string message) : base(message)
        {
        }
    }
}
