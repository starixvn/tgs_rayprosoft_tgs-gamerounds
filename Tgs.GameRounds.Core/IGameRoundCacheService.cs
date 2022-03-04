namespace Tgs.GameRounds.Core
{
    public interface IGameRoundCacheService
    {
        ValueTask<Guid?> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);
    }
}
