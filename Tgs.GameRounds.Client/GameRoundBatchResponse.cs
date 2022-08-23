using System.Net;

namespace Tgs.GameRounds.Client;

public record GameRoundBatchResponse(
	IEnumerable<Guid> RoundIds,
	 bool Success = false,
	 HttpStatusCode HttpStatusCode = HttpStatusCode.InternalServerError);
