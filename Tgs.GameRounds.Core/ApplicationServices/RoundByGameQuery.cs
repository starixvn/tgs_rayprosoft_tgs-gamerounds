namespace Tgs.GameRounds.Core.ApplicationServices;

public record RoundByGameQuery(
	Guid GameProviderId,
	int GameId,
	Guid WalletId,
	string ExternalRoundId,
	DateTimeOffset? ExternalCreatedOn);
