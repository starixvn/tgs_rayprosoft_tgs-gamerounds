using Microsoft.EntityFrameworkCore;

using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories;

internal class GameRepository : IGame
{
    private readonly TgsContext _context;

    public GameRepository(
        TgsContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public ValueTask<string> GetExternalGameIdAsync(
        int gameId,
        CancellationToken cancellationToken = default)
        => _context.Games.AsQueryable()
            .Where(x => x.Id == gameId)
            .Select(x => x.ExternalId.Trim())
            .ToAsyncEnumerable()
            .SingleAsync(cancellationToken: cancellationToken);

    public ValueTask<int> GetGameIdAsync(
        Guid gameProviderId,
        string externalGameId,
        CancellationToken cancellationToken = default)
        => _context.Games.AsQueryable()
            .Where(x => x.GameProviderId == gameProviderId && x.ExternalId == externalGameId)
            .Select(x => x.Id)
            .ToAsyncEnumerable()
            .SingleAsync(cancellationToken: cancellationToken);
}
