using System.Net;

namespace Tgs.GameRounds.Client;

public record GameRoundResponse(
	 bool Success = false,
	 HttpStatusCode HttpStatusCode = HttpStatusCode.InternalServerError);
