namespace Tgs.GameRounds.Core
{
    public class GameLockedOptions
    {
        public int LockSeconds { get; init; } = 60;

        public string LocksRedisKey { get; init; } = "gameroundlock:{0}:{1}:{2}:{3}";
    }
}
