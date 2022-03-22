namespace Tgs.GameRounds.Core
{
    public interface IGameRepository
    {
        public ValueTask<int?> GetGameIdAsync(Guid gameProviderId, string externalGameId);
    }
}
