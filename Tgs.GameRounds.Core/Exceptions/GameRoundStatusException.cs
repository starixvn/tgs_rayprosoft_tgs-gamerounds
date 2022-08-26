using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundStatusException : Exception
{
	public GameRoundStatusException(GameRoundStatus status) : base($"This order has the wrong status.({status})")
	{
	}
}
