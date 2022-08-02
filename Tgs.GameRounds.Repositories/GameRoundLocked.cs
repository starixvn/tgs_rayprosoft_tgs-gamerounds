using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.Repositories
{
    internal class GameRoundLocked : IGameRoundLocked
    {
        private readonly IDatabase _database;
        private readonly TimeSpan _expiry;
        private readonly string _key;

        public GameRoundLocked(
            IDatabase database,
            IOptions<GameLockedOptions> options)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _expiry = TimeSpan.FromSeconds(options.Value.LockSeconds);
            _key = options.Value.LocksRedisKey;
        }

        public Task<bool> ExistAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default)
        {
            string key = string.Format(_key, gameProviderId, externalRoundId, gameId, walletId).ToLower();

            try
            {
                return _database.KeyExistsAsync(key);
            }
            catch
            {
                return Task.FromResult(true);
            }
        }

        public Task<bool> LockingAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default)
        {
            string key = string.Format(_key, gameProviderId, externalRoundId, gameId, walletId).ToLower();
            var value = JsonSerializer.Serialize(new
            {
                GameProviderId = gameProviderId,
                ExternalRoundId = externalRoundId,
                GameId = gameId,
                WalletId = walletId,
                LockTime = DateTime.UtcNow.ToString("O")
            });
            return _database.StringSetAsync(
                key: key,
                value: value,
                expiry: _expiry,
                when: When.NotExists);
        }

        public async ValueTask UnlockAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default)
        {
            string key = string.Format(_key, gameProviderId, externalRoundId, gameId, walletId).ToLower();
            _ = await _database.KeyDeleteAsync(
                key: key,
                flags: CommandFlags.FireAndForget).ConfigureAwait(false);
        }
    }
}
