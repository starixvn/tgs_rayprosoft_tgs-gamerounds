namespace Tgs.GameRounds.Core.ApplicationServices;

public record GetGameRoundByGameQuery(
    Guid GameProviderId,
    string ExternalRoundId,
    int GameId,
    Guid WalletId);
