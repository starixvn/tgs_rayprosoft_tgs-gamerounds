using System.Net;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

public record GameRoundDetailDataResponse(
	 bool Success = false,
	 HttpStatusCode HttpStatusCode = HttpStatusCode.InternalServerError,
	 GameRoundDetailViewModel? Data = default);
