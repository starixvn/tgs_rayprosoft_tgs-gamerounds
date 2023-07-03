using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundStatusException : Exception
{
    public GameRoundStatusException(Guid roundId, GameRoundStatus status) : base($"This order has the wrong status.({roundId} | {status})")
    {
    }
}
