using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories;

internal class GameProviderRepository : IGameProviderRepository
{
    private readonly TgsContext _context;

    public GameProviderRepository(
        TgsContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public ValueTask<string> GetCode(
        Guid id,
        CancellationToken cancellationToken = default)
        => _context.GameProviders.AsQueryable()
        .Where(x => x.Id == id)
        .Select(x => x.Code.Trim())
        .ToAsyncEnumerable()
        .SingleAsync(cancellationToken: cancellationToken);

    public IAsyncEnumerable<Guid> GetIds(
        CancellationToken cancellationToken = default)
        => _context.GameProviders.AsQueryable()
        .Select(x => x.Id)
        .ToAsyncEnumerable();
}
