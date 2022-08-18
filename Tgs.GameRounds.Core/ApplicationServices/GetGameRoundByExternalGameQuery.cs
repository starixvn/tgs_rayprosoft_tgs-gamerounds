namespace Tgs.GameRounds.Core.ApplicationServices;

public record GetGameRoundByExternalGameQuery(Guid GameProviderId, string ExternalRoundId, string ExternalGameId, Guid WalletId);
