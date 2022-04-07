using StackExchange.Redis;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.Repositories
{
    internal class GameProviderRedisRepository : IGameProvider
    {
        private readonly IDatabase _database;
        private readonly TimeSpan _expiry;
        private readonly IGameProvider _gameProvider;
        private readonly string _key = "gameround:gameprovider:{0}";

        public GameProviderRedisRepository(IDatabase database, IGameProvider gameProvider, TimeSpan expiry)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _expiry = expiry;
        }

        public async ValueTask<string> GetGameProviderCodeAsync(Guid gameProviderId)
        {
            var key = string.Format(_key, gameProviderId);
            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var gameProviderCode = await _gameProvider.GetGameProviderCodeAsync(gameProviderId).ConfigureAwait(false);
                _ = await _database.StringSetAsync(key, gameProviderCode, _expiry);
                return gameProviderCode;
            }

            var value = await _database.StringGetAsync(key).ConfigureAwait(false);

            return value.ToString();
        }
    }
}
