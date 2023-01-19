using StackExchange.Redis;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.Repositories;

internal class GameProviderCacheRepository : IGameProvider
{
    private const string KeyGameProvider = "gameround:gameprovider:{0}";
    private readonly IDatabase _database;
    private readonly TimeSpan _expiry;
    private readonly IGameProvider _gameProvider;

    public GameProviderCacheRepository(IGameProvider gameProvider, IDatabase database, TimeSpan expiry)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _expiry = expiry;
    }

    public async ValueTask<string> GetGameProviderCodeAsync(
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
    {
        var key = string.Format(KeyGameProvider, gameProviderId);
        if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
        {
            var gameProviderCode = await _gameProvider.GetGameProviderCodeAsync(
                gameProviderId: gameProviderId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            _ = await _database.StringSetAsync(
                key: key,
                value: gameProviderCode,
                expiry: _expiry,
                when: When.NotExists);

            return gameProviderCode;
        }

        var value = await _database.StringGetAsync(key).ConfigureAwait(false);

        return value.ToString();
    }
}
