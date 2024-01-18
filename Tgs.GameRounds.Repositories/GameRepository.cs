using Microsoft.EntityFrameworkCore;

using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories;

internal class GameRepository(
    TgsContext context) : IGameRepository
{
    public ValueTask<string> GetExternalIdAsync(
        int id,
        CancellationToken cancellationToken = default)
        => context.Games.AsQueryable()
            .Where(x => x.Id == id)
            .Select(x => x.ExternalId.Trim())
            .ToAsyncEnumerable()
            .SingleAsync(cancellationToken: cancellationToken);

    public ValueTask<int> GetIdAsync(
        Guid gameProviderId,
        string externalId,
        CancellationToken cancellationToken = default)
        => context.Games.AsQueryable()
            .Where(x => x.GameProviderId == gameProviderId && x.ExternalId == externalId)
            .Select(x => x.Id)
            .ToAsyncEnumerable()
            .SingleAsync(cancellationToken: cancellationToken);
}
