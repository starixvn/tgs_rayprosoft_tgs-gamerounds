namespace Tgs.GameRounds.Core.Exceptions;

public class GameRoundCompanyOrderDuplicateException(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId) : Exception($"This company has a duplicate order.({gameProviderId} | {externalRoundId} | {gameId} | {walletId})")
{
}
