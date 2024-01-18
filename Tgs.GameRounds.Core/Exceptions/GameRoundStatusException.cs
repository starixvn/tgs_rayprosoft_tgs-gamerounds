using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundStatusException(Guid roundId, GameRoundStatus status) : Exception($"This order has the wrong status.({roundId} | {status})")
{
}
