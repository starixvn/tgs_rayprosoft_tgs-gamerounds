namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundLockedException(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId) : Exception($"This order is locked.({gameProviderId} | {externalRoundId} | {gameId} | {walletId})")
{
}
