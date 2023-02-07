using System.Text.Json;
using Microsoft.Extensions.Options;

using StackExchange.Redis;

using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.Repositories;

internal class GameRoundLocked : IGameRoundLocked
{
    private readonly IDatabase _database;
    private readonly GameLockedOptions _gameLockedOptions;

    public GameRoundLocked(
        IDatabase database,
        IOptions<GameLockedOptions> options)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _gameLockedOptions = options.Value;
    }

    public Task<bool> LockingAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        CancellationToken cancellationToken = default)
        => _database
        .StringSetAsync(
            key: GetKey(gameProviderId, externalRoundId, gameId, walletId),
            value: JsonSerializer.Serialize(new
            {
                GameProviderId = gameProviderId,
                ExternalRoundId = externalRoundId,
                GameId = gameId,
                WalletId = walletId,
                LockTime = DateTime.UtcNow.ToString("O")
            }),
            expiry: TimeSpan.FromSeconds(_gameLockedOptions.LockSeconds),
            when: When.NotExists);

    public Task UnlockAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default)
        => _database
        .KeyDeleteAsync(
            key: GetKey(gameProviderId, externalRoundId, gameId, walletId),
            flags: CommandFlags.FireAndForget);

    private string GetKey(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
        => string.Format(_gameLockedOptions.LocksRedisKey, gameProviderId, externalRoundId, gameId, walletId);
}
