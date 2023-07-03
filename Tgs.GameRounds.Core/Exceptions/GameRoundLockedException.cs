namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundLockedException : Exception
{
    public GameRoundLockedException(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
        : base($"This order is locked.({gameProviderId} | {externalRoundId} | {gameId} | {walletId})")
    {
    }
}
