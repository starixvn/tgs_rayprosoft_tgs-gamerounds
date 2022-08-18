namespace Tgs.GameRounds.Core.ApplicationServices;

public record UpdateRoundDetailCommand(Guid RoundId, string AdditionalData);
