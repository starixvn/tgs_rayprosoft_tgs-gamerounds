namespace Tgs.GameRounds.Core.ApplicationServices;

public record GameRoundByExternalGameQuery(Guid GameProviderId, string ExternalRoundId, string ExternalGameId, Guid WalletId);
