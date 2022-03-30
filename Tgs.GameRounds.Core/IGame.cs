namespace Tgs.GameRounds.Core
{
    public interface IGame
    {
        public ValueTask<string> GetExternalGameIdAsync(int gameId);

        public ValueTask<int> GetGameIdAsync(Guid gameProviderId, string externalGameId);
    }
}
