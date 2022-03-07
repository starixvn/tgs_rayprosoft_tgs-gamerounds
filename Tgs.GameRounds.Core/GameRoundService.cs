using System.Text.Json;
using StackExchange.Redis;

namespace Tgs.GameRounds.Core
{
    internal class GameRoundService : IGameRoundService
    {
        private readonly IDatabase _database;
        private readonly string _existKey = "gameround:exist:{0}";
        private readonly TimeSpan _expiry;
        private readonly string _externalKey = "gameround:external:{0}:{1}:{2}:{3}";
        private readonly IGameRoundRepository _gameRoundRepository;

        public GameRoundService(IDatabase database, IGameRoundRepository gameRoundRepository, TimeSpan expiry)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
            _expiry = expiry;
        }

        public async ValueTask<bool> ExistAsync(Guid roundId)
        {
            string key = string.Format(_existKey, roundId);

            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var round = await _gameRoundRepository.GetAsync(roundId).ConfigureAwait(false);
                if (round == null)
                {
                    return false;
                }
                await _database.StringSetAsync(key, JsonSerializer.Serialize(new
                {
                    RoundId = round.RoundId,
                    LogOn = DateTimeOffset.UtcNow
                }), _expiry).ConfigureAwait(false);
                return true;
            }
            return true;
        }

        public async ValueTask<Guid?> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
        {
            string key = string.Format(_externalKey, gameProviderId, externalRoundId, gameId, walletId);

            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var round = await _gameRoundRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);
                if (round == null)
                {
                    return null;
                }
                await _database.StringSetAsync(key, round.RoundId.ToString(), _expiry).ConfigureAwait(false);
                return round.RoundId;
            }
            return Guid.TryParse(await _database.StringGetAsync(key).ConfigureAwait(false), out Guid roundId) ? roundId : null;
        }
    }
}
