namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundCompanyOrderDuplicateException : Exception
{
    public GameRoundCompanyOrderDuplicateException(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
        : base($"This company has a duplicate order.({gameProviderId} | {externalRoundId} | {gameId} | {walletId})")
    {
    }
}
