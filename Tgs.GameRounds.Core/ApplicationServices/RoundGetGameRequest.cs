using MediatR;

namespace Tgs.GameRounds.Core.ApplicationServices;

public record RoundGetGameRequest(
	Guid GameProviderId,
	int GameId,
	Guid WalletId,
	string ExternalRoundId,
	DateTimeOffset? ExternalCreatedOn) : IRequest<RoundInfoResponse?>;
