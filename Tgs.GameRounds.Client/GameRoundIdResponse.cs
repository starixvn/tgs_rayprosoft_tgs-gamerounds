using System.Net;

namespace Tgs.GameRounds.Client;

public record GameRoundIdResponse(
	 bool Success = false,
	 HttpStatusCode HttpStatusCode = HttpStatusCode.InternalServerError,
	 Guid? RoundId = default);
