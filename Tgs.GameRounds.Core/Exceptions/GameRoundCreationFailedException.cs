namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundCreationFailedException(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId) : Exception($"This GameRound failed to create an order.({gameProviderId} | {externalRoundId} | {gameId} | {walletId})")
{
}
