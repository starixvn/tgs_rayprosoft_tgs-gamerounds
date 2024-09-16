using MediatR;

namespace Tgs.GameRounds.Core.ApplicationServices;

public record RoundGetExternalGameRequest(
	Guid GameProviderId,
	Guid WalletId,
	string ExternalRoundId,
	string ExternalGameId,
	DateTimeOffset? ExternalCreatedOn) : IRequest<RoundInfoResponse?>;
