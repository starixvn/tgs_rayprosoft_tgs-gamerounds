using MediatR;

namespace Tgs.GameRounds.Core.ApplicationServices;

public record RoundGetRequest(
	Guid RoundId,
	DateTimeOffset? ExternalCreatedOn) : IRequest<RoundInfoResponse?>;
