namespace Tgs.GameRounds.Core.ApplicationServices;

public record RoundByExternalGameQuery(
	Guid GameProviderId,
	Guid WalletId,
	string ExternalRoundId,
	string ExternalGameId,
	DateTimeOffset? ExternalCreatedOn);
