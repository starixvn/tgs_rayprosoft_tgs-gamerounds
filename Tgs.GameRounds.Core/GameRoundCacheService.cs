using StackExchange.Redis;

namespace Tgs.GameRounds.Core
{
    internal class GameRoundCacheService : IGameRoundCacheService
    {
        private readonly IDatabase _database;
        private readonly TimeSpan _expiry;
        private readonly string _externalKey = "gameround:external:{0}:{1}:{2}:{3}";
        private readonly IGameRoundRepository _gameRoundRepository;

        public GameRoundCacheService(IDatabase database, IGameRoundRepository gameRoundRepository, TimeSpan expiry)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
            _expiry = expiry;
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
                await SetExternalAsync(gameProviderId, externalRoundId, gameId, walletId, round.RoundId).ConfigureAwait(false);
                return round.RoundId;
            }
            return Guid.TryParse(await _database.StringGetAsync(key).ConfigureAwait(false), out Guid roundId) ? roundId : null;
        }

        private async ValueTask SetExternalAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, Guid roundId)
        {
            string key = string.Format(_externalKey, gameProviderId, externalRoundId, gameId, walletId);
            await _database.StringSetAsync(key, roundId.ToString(), _expiry).ConfigureAwait(false);
        }
    }
}
