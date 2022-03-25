namespace Tgs.GameRounds.Core
{
    internal class NoopGame : IGame
    {
        public ValueTask<string> GetExternalGameIdAsync(int gameId) => throw new NotSupportedException();

        public ValueTask<int> GetGameIdAsync(Guid gameProviderId, string externalGameId) => throw new NotSupportedException();
    }
}
