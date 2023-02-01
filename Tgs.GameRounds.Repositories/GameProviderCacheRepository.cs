using Microsoft.Extensions.Caching.Memory;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.Repositories;

internal class GameProviderCacheRepository : IGameProviderRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IGameProviderRepository _repository;

    public GameProviderCacheRepository(
        IGameProviderRepository repository,
        IMemoryCache memoryCache)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async ValueTask<string> GetCode(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var key = string.Format("gamerounds:gameprovider:get:code:{0}", id);
        if (!_memoryCache.TryGetValue(key, out string? code))
        {
            code = await _repository
                .GetCode(
                    id: id,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(code))
            {
                return _memoryCache.Set(
                    key: key,
                    value: code);
            }
        }
        return code!;
    }

    public IAsyncEnumerable<Guid> GetIds(
        CancellationToken cancellationToken = default)
        => _repository.GetIds(cancellationToken);
}
