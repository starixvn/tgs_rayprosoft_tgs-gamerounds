namespace Tgs.GameRounds.Core.ApplicationServices;
public record GetListByRoundIdQuery(
    Guid GameProviderId,
    string ExternalRoundId,
    string ExternalGameId);
