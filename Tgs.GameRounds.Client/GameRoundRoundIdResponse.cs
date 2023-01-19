using System.Net;

namespace Tgs.GameRounds.Client;

public record GameRoundRoundIdResponse(
     bool Success = false,
     HttpStatusCode HttpStatusCode = HttpStatusCode.InternalServerError,
     Guid? RoundId = default);
