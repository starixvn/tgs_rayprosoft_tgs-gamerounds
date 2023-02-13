namespace Tgs.GameRounds.Core;

public class GameLockedOptions
{
    public double LockSeconds { get; init; }

    public string LocksRedisKey { get; init; } = default!;
}
