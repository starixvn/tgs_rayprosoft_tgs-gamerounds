namespace Tgs.GameRounds.Core
{
    public interface IGameRoundCacheService
    {
        ValueTask<bool> ExistAsync(Guid roundId);

        ValueTask<Guid?> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);
    }
}
