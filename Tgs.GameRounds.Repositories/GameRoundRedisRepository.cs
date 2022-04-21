using System.Text.Json;
using StackExchange.Redis;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Repositories
{
    internal class GameRoundRedisRepository : IGameRound
    {
        private const string KeyExist = "gameround:exist:{0}";
        private const string KeyExternal = "gameround:external:{0}:{1}:{2}:{3}";
        private const string KeyLock = "gameround:lock:{0}:{1}:{2}:{3}";
        private readonly IDatabase _database;
        private readonly TimeSpan _expiry;
        private readonly IGameRound _gameRound;

        public GameRoundRedisRepository(IDatabase database, IGameRound gameRound, TimeSpan expiry)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _expiry = expiry;
        }

        public ValueTask<Guid> AddRoundAsync(AddRound source)
            => _gameRound.AddRoundAsync(source);

        public async ValueTask<bool> ExistAsync(Guid roundId)
        {
            string key = string.Format(KeyExist, roundId).ToLower();

            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var round = await _gameRound.GetAsync(roundId).ConfigureAwait(false);
                if (round == null)
                {
                    return false;
                }
                _ = await _database.StringSetAsync(key, JsonSerializer.Serialize(new
                {
                    round.RoundId,
                    LogOn = DateTimeOffset.UtcNow
                }), _expiry).ConfigureAwait(false);
                return true;
            }
            return true;
        }

        public ValueTask<GameRoundInfo?> GetAsync(Guid roundId)
            => _gameRound.GetAsync(roundId);

        public ValueTask<GameRoundInfo?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
            => _gameRound.GetAsync(gameProviderId, externalRoundId, gameId, walletId);

        public async ValueTask<Guid?> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
        {
            string key = string.Format(KeyExternal, gameProviderId, externalRoundId, gameId, walletId).ToLower();

            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var round = await _gameRound.GetAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);
                if (round == null)
                {
                    return null;
                }
                _ = await _database.StringSetAsync(key, round.RoundId.ToString(), _expiry).ConfigureAwait(false);
                return round.RoundId;
            }
            return Guid.TryParse(await _database.StringGetAsync(key).ConfigureAwait(false), out Guid roundId) ? roundId : null;
        }

        public async ValueTask<bool> LockAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
        {
            string key = string.Format(KeyLock, gameProviderId, externalRoundId, gameId, walletId).ToLower();
            return !await _database.KeyExistsAsync(key).ConfigureAwait(false)
                && await _database.StringSetAsync(key, Guid.Empty.ToString(), _expiry).ConfigureAwait(false);
        }

        public ValueTask<Guid?> NewRoundAsync(NewRound round)
            => _gameRound.NewRoundAsync(round);

        public async ValueTask UnLockAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
        {
            string key = string.Format(KeyLock, gameProviderId, externalRoundId, gameId, walletId).ToLower();
            _ = await _database.KeyDeleteAsync(key).ConfigureAwait(false);
        }

        public ValueTask UpdateClientStatusAsync(Guid roundId, GameRoundClientStatus status)
            => _gameRound.UpdateClientStatusAsync(roundId, status);

        public ValueTask UpdateRoundAsync(UpdateRound round)
            => _gameRound.UpdateRoundAsync(round);

        public ValueTask UpdateRoundClosedAsync(UpdateRoundClosed source)
            => _gameRound.UpdateRoundClosedAsync(source);

        public ValueTask UpdateRoundDetailAsync(Guid roundId, string additionalData)
            => _gameRound.UpdateRoundDetailAsync(roundId, additionalData);

        public ValueTask UpdateRoundOpenAsync(UpdateRoundOpen source)
             => _gameRound.UpdateRoundOpenAsync(source);

        public ValueTask UpdateRoundReopenAsync(UpdateRoundReopen source)
            => _gameRound.UpdateRoundReopenAsync(source);

        public ValueTask UpdateStatusAsync(Guid roundId, GameRoundStatus status, DateTimeOffset? externalClosedOn = null)
            => _gameRound.UpdateStatusAsync(roundId, status, externalClosedOn);
    }
}
