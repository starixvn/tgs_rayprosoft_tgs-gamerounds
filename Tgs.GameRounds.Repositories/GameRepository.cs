using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

using Tgs.GameRounds.Core;
using Tgs.GameRounds.Entities;

namespace Tgs.GameRounds.Repositories;

internal class GameRepository(
	TgsContext context) : IGameRepository
{
	public async ValueTask<string> GetExternalIdAsync(int id, CancellationToken cancellationToken = default)
		=> await context.Games
		.Where(x => x.Id == id)
		.Select(x => x.ExternalId)
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(id.ToString(), "This game identification code does not exist.");

	public async ValueTask<int> GetIdAsync(
		Guid gameProviderId,
		string externalGameId,
		CancellationToken cancellationToken = default)
		=> await context.Games
		.Where(x => x.GameProviderId == gameProviderId && x.ExternalId == externalGameId)
		.Select(x => (int?)x.Id)
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(
			paramName: new JsonObject
			{
				["GameProviderId"] = gameProviderId,
				["ExternalGameId"] = externalGameId
			}.ToJsonString(),
			message: "The game provider game identifier does not exist.");
}
