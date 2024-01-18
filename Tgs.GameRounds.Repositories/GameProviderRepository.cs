using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories;

internal class GameProviderRepository(
    TgsContext context) : IGameProviderRepository
{
    public ValueTask<string> GetCode(
        Guid id,
        CancellationToken cancellationToken = default)
        => context.GameProviders.AsQueryable()
        .Where(x => x.Id == id)
        .Select(x => x.Code.Trim())
        .ToAsyncEnumerable()
        .SingleAsync(cancellationToken: cancellationToken);
}
