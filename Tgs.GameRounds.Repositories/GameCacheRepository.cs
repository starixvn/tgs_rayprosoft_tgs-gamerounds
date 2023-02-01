using Microsoft.Extensions.Caching.Memory;

using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.Repositories;

internal class GameCacheRepository : IGameRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IGameRepository _repository;

    public GameCacheRepository(
        IGameRepository repository,
        IMemoryCache memoryCache)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async ValueTask<string> GetExternalIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var key = string.Format("gamerounds:game:get:externalid:{0}", id);
        if (!_memoryCache.TryGetValue(key, out string? externalId))
        {
            externalId = await _repository
                .GetExternalIdAsync(
                    id: id,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(externalId))
            {
                return _memoryCache.Set(
                    key: key,
                    value: externalId,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
            }
        }
        return externalId!;
    }

    public async ValueTask<int> GetIdAsync(
        Guid gameProviderId,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var key = string.Format("gamerounds:game:get:id:{0}:{1}", gameProviderId, externalId);
        if (!_memoryCache.TryGetValue(key, out int? id))
        {
            id = await _repository
                        .GetIdAsync(
                           gameProviderId: gameProviderId,
                           externalId: externalId,
                           cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
            if (id.HasValue)
            {
                return _memoryCache.Set(
                    key: key,
                    value: id.Value,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
            }
        }
        return id!.Value;
    }
}
