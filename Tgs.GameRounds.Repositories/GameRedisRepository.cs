using System.Text.Json;
using StackExchange.Redis;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.Repositories
{
    internal class GameRedisRepository : IGame
    {
        private readonly IDatabase _database;
        private readonly TimeSpan _expiry;
        private readonly IGame _game;
        private readonly string _keyExternalGameId = "gameround:game:externalgameid:{0}:{1}";
        private readonly string _keyGameId = "gameround:game:gameid:{0}";

        public GameRedisRepository(IDatabase database, IGame game, TimeSpan expiry)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _expiry = expiry;
        }

        public async ValueTask<string> GetExternalGameIdAsync(int gameId)
        {
            var key = string.Format(_keyGameId, gameId);
            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var externalGameId = await _game.GetExternalGameIdAsync(gameId).ConfigureAwait(false);
                _ = await _database.StringSetAsync(key, externalGameId, _expiry);
                return externalGameId;
            }

            var value = await _database.StringGetAsync(key).ConfigureAwait(false);

            return value.ToString();
        }

        public async ValueTask<int> GetGameIdAsync(Guid gameProviderId, string externalGameId)
        {
            var key = string.Format(_keyExternalGameId, gameProviderId, externalGameId);
            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var gameId = await _game.GetGameIdAsync(gameProviderId, externalGameId).ConfigureAwait(false);
                _ = await _database.StringSetAsync(key, gameId, _expiry);
                return gameId;
            }

            return JsonSerializer.Deserialize<int>(await _database.StringGetAsync(key).ConfigureAwait(false));
        }
    }
}
