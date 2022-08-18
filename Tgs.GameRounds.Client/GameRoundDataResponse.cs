using System.Net;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

public record GameRoundDataResponse(
	 bool Success = false,
	 HttpStatusCode HttpStatusCode = HttpStatusCode.InternalServerError,
	 GameRoundViewModel? Data = default);
