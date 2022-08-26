using StackExchange.Redis;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.Repositories;

internal class GameCacheRepository : IGame
{
	private const string KeyExternalGameId = "gameround:game:externalgameid:{0}:{1}";
	private const string KeyGameId = "gameround:game:gameid:{0}";
	private readonly IDatabase _database;
	private readonly TimeSpan _expiry;
	private readonly IGame _game;

	public GameCacheRepository(IGame game, IDatabase database, TimeSpan expiry)
	{
		_database = database ?? throw new ArgumentNullException(nameof(database));
		_game = game ?? throw new ArgumentNullException(nameof(game));
		_expiry = expiry;
	}

	public async ValueTask<string> GetExternalGameIdAsync(
		int gameId,
		CancellationToken cancellationToken = default)
	{
		var key = string.Format(KeyGameId, gameId);
		if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
		{
			var externalGameId = await _game.GetExternalGameIdAsync(
				gameId: gameId,
				cancellationToken: cancellationToken).ConfigureAwait(false);

			_ = await _database.StringSetAsync(
				key: key,
				value: externalGameId,
				expiry: _expiry,
				when: When.NotExists,
				flags: CommandFlags.FireAndForget);

			return externalGameId;
		}

		var value = await _database.StringGetAsync(key).ConfigureAwait(false);

		return value.ToString();
	}

	public async ValueTask<int> GetGameIdAsync(
		Guid gameProviderId,
		string externalGameId,
		CancellationToken cancellationToken = default)
	{
		var key = string.Format(KeyExternalGameId, gameProviderId, externalGameId);
		if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
		{
			var gameId = await _game.GetGameIdAsync(
				gameProviderId: gameProviderId,
				externalGameId: externalGameId,
				cancellationToken: cancellationToken).ConfigureAwait(false);

			_ = await _database.StringSetAsync(
				key: key,
				value: gameId,
				expiry: _expiry,
				when: When.NotExists,
				flags: CommandFlags.FireAndForget);

			return gameId;
		}
		var value = await _database.StringGetAsync(key).ConfigureAwait(false);

		return int.Parse(value!);
	}
}
