namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundCreationFailedException : Exception
{
    public GameRoundCreationFailedException(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
        : base($"This GameRound failed to create an order.({gameProviderId} | {externalRoundId} | {gameId} | {walletId})")
    {
    }
}
