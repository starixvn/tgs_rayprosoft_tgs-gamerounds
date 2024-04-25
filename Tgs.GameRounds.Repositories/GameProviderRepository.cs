using Microsoft.EntityFrameworkCore;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Entities;

namespace Tgs.GameRounds.Repositories;

internal class GameProviderRepository(
	TgsContext context) : IGameProviderRepository
{
	public async ValueTask<string> GetCode(Guid id, CancellationToken cancellationToken = default)
		=> await context.GameProviders
		.Where(x => x.Id == id)
		.Select(x => x.Code.Trim())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(id.ToString(), "This game provider does not exist.");
}
