namespace Tgs.GameRounds.Core.ApplicationServices;

public record GameRoundByGameQuery(
    Guid GameProviderId,
    string ExternalRoundId,
    int GameId,
    Guid WalletId);
