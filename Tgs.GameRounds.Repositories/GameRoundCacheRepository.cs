using Microsoft.Extensions.Caching.Memory;

using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Repositories;

internal class GameRoundCacheRepository : IGameRoundRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IGameRoundRepository _repository;
    private readonly TimeSpan _absoluteExpirationRelativeToNow;

    public GameRoundCacheRepository(
        IGameRoundRepository repository,
        IMemoryCache memoryCache)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
    }

    public ValueTask<Guid?> AddCompanyRoundAsync(
        AddCompanyRound source,
        CancellationToken cancellationToken = default)
        => _repository.AddCompanyRoundAsync(
            source,
            cancellationToken);

    public ValueTask<Guid?> AddRoundAsync(
        AddRound source,
        CancellationToken cancellationToken = default)
        => _repository.AddRoundAsync(
            source,
            cancellationToken);

    public ValueTask AdjustClosedRoundAsync(
        Guid id,
        AdjustClosedRound source,
        CancellationToken cancellationToken = default)
        => _repository.AdjustClosedRoundAsync(
            id: id,
            source: source,
            cancellationToken: cancellationToken);

    public async ValueTask<bool> ExistAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var key = string.Format("gamerounds:gameround:exist:{0}", id);
        if (!_memoryCache.TryGetValue(key, out _))
        {
            var exist = await _repository
                 .ExistAsync(
                     id: id,
                     cancellationToken: cancellationToken)
                 .ConfigureAwait(false);
            return exist
                ? _memoryCache.Set(
                    key: key,
                    value: exist,
                    absoluteExpirationRelativeToNow: _absoluteExpirationRelativeToNow)
                : exist;
        }
        return true;
    }

    public ValueTask<GameRoundInfo?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _repository.GetAsync(
            id,
            cancellationToken: cancellationToken);

    public ValueTask<GameRoundInfo?> GetAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        CancellationToken cancellationToken = default)
        => _repository.GetAsync(
            gameProviderId: gameProviderId,
            externalRoundId: externalRoundId,
            gameId: gameId,
            walletId: walletId,
            cancellationToken: cancellationToken);

    public ValueTask<GameRoundDetailInfo?> GetRoundDetailAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _repository.GetRoundDetailAsync(
            id: id,
            cancellationToken: cancellationToken);

    public async ValueTask<Guid?> GetRoundIdAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        CancellationToken cancellationToken = default)
    {
        var key = string.Format("gamerounds:gameround:get:roundid:{0}:{1}:{2}:{3}", gameProviderId, externalRoundId, gameId, walletId);
        if (!_memoryCache.TryGetValue(key, out Guid? roundId))
        {
            roundId = await _repository
                .GetRoundIdAsync(
                    gameProviderId: gameProviderId,
                    externalRoundId: externalRoundId,
                    gameId: gameId,
                    walletId: walletId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (roundId.HasValue)
            {
                _ = _memoryCache.Set(
                     key: key,
                     value: roundId.Value,
                     absoluteExpirationRelativeToNow: _absoluteExpirationRelativeToNow);
            }
        }
        return roundId;

    }

    public ValueTask<GameRoundStatus> GetStatusAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _repository.GetStatusAsync(
            id: id,
            cancellationToken: cancellationToken);

    public ValueTask ManualCloseRoundAsync(
        Guid id,
        ManualCloseRound source,
        CancellationToken cancellationToken = default)
        => _repository.ManualCloseRoundAsync(
            id: id,
            source: source,
            cancellationToken: cancellationToken);

    public ValueTask UpdateRoundClosedAsync(
        Guid id,
        UpdateRoundClosed source,
        CancellationToken cancellationToken = default)
        => _repository.UpdateRoundClosedAsync(
            id: id,
            source: source,
            cancellationToken: cancellationToken);

    public ValueTask UpdateRoundDetailAsync(
        Guid id,
        string additionalData,
        CancellationToken cancellationToken = default)
        => _repository.UpdateRoundDetailAsync(
            id: id,
            additionalData: additionalData,
            cancellationToken: cancellationToken);

    public ValueTask UpdateRoundOpenAsync(
        Guid id,
        UpdateRoundOpen source,
        CancellationToken cancellationToken = default)
         => _repository.UpdateRoundOpenAsync(
             id: id,
             source: source,
             cancellationToken: cancellationToken);

    public ValueTask UpdateRoundReopenAsync(
        Guid id,
        UpdateRoundReopen source,
        CancellationToken cancellationToken = default)
        => _repository.UpdateRoundReopenAsync(
            id: id,
            source: source,
            cancellationToken: cancellationToken);
}
