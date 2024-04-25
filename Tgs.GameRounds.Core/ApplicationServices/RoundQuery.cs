namespace Tgs.GameRounds.Core.ApplicationServices;

public record RoundQuery(
	Guid RoundId,
	DateTimeOffset? ExternalCreatedOn);
