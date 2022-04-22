using System.Net;
using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Client
{
    public record GameRoundRoundStatusResponse(
         bool Success = false,
         HttpStatusCode HttpStatusCode = HttpStatusCode.InternalServerError,
         GameRoundStatus? RoundStatus = default);
}
