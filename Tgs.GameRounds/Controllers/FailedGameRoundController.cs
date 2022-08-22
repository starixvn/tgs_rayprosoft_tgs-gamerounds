using Microsoft.AspNetCore.Mvc;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Controllers;

[Route("[controller]")]
[ApiController]
public class FailedGameRoundController : ControllerBase
{
	/// <summary>
	/// Adds the asynchronous.
	/// </summary>
	/// <param name="failedGameRound">The repository.</param>
	/// <param name="model">The model.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	[HttpPost(Name = "FailedGameRound_Add")]
	public async ValueTask AddAsync(
		[FromServices] IFailedGameRound failedGameRound,
		AddFailedGameRoundViewModel model,
		CancellationToken cancellationToken)
	{
		var add = new AddFailedGameRound(
			model.GameProviderId,
			model.GameProviderCode,
			model.ExternalRoundId,
			model.PlayerCode,
			model.GameId,
			model.ExternalGameId,
			model.Detail,
			FailedGameRoundAutoStatus.Manual,
			FailedGameRoundProcesStatus.Process);

		await failedGameRound
				.AddAsync(
					add,
					cancellationToken)
			   .ConfigureAwait(false);
	}

	/// <summary>
	/// Gets the asynchronous.
	/// </summary>
	/// <param name="failedGameRound">The repository.</param>
	/// <param name="gameProviderId">The game provider identifier.</param>
	/// <param name="externalRoundId">The external round identifier.</param>
	/// <param name="gameId">The game identifier.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	[HttpGet("external", Name = "FailedGameRound_Get")]
	public async ValueTask<FailedGameRoundInfoViewModel?> GetAsync(
		[FromServices] IFailedGameRound failedGameRound,
		Guid gameProviderId,
		string externalRoundId,
		int gameId,
		string playerCode,
		CancellationToken cancellationToken)
	{
		var info = await failedGameRound
				.GetAsync(
					gameProviderId,
					externalRoundId,
					gameId,
					playerCode,
					cancellationToken).ConfigureAwait(false);

		var result = default(FailedGameRoundInfoViewModel);

		if (info is not null)
		{
			result = new FailedGameRoundInfoViewModel()
			{
				FailedGameRoundId = info.FailedGameRoundId,
				GameProviderId = info.GameProviderId,
				GameProviderCode = info.GameProviderCode,
				ExternalRoundId = info.ExternalRoundId,
				PlayerCode = info.PlayerCode,
				GameId = info.GameId,
				ExternalGameId = info.ExternalGameId,
				Detail = info.Detail,
				AutoStatus = info.AutoStatus,
				ProcesStatus = info.ProcesStatus,
				CreatedOn = info.CreatedOn,
				UpdatedOn = info.UpdatedOn
			};
		}

		return result;
	}
}
