using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories;

internal class GameProviderRepository : IGameProvider
{
	private readonly TgsContext _context;

	public GameProviderRepository(
		TgsContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
	}

	public ValueTask<string> GetGameProviderCodeAsync(
		Guid gameProviderId,
		CancellationToken cancellationToken = default)
		=> _context.GameProviders.AsQueryable()
			.Where(x => x.Id == gameProviderId)
			.Select(x => x.Code.Trim())
			.ToAsyncEnumerable()
			.SingleAsync(cancellationToken: cancellationToken);
}
