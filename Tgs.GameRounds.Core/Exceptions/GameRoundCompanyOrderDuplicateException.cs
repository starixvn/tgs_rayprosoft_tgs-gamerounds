namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundCompanyOrderDuplicateException : Exception
{
    public GameRoundCompanyOrderDuplicateException() : base("This company has a duplicate order.")
    {
    }

    public GameRoundCompanyOrderDuplicateException(string message) : base(message)
    {
    }
}
