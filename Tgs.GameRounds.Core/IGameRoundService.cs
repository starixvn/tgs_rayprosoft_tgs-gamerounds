namespace Tgs.GameRounds.Core
{
    public interface IGameRoundService
    {
        ValueTask<bool> ExistAsync(Guid roundId);

        ValueTask<Guid?> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<bool> LockAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask UnLockAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);
    }
}
