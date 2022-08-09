using System.Text.Json;
using StackExchange.Redis;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Repositories
{
    internal class GameRoundCacheRepository : IGameRound
    {
        private const string KeyExist = "gameround:exist:{0}";
        private const string KeyRoundId = "gameround:roundid:{0}:{1}:{2}:{3}";
        private readonly IDatabase _database;
        private readonly TimeSpan _expiry;
        private readonly IGameRound _gameRound;

        public GameRoundCacheRepository(
            IGameRound gameRound,
            IDatabase database,
            TimeSpan expiry)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _expiry = expiry;
        }

        public ValueTask<Guid?> AddRoundAsync(
            AddRound source,
            CancellationToken cancellationToken = default)
            => _gameRound.AddRoundAsync(
                source,
                cancellationToken);

        public async ValueTask<bool> ExistAsync(
            Guid roundId,
            CancellationToken cancellationToken = default)
        {
            string key = string.Format(KeyExist, roundId).ToLower();

            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var round = await _gameRound.GetAsync(
                    roundId: roundId,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (round == null)
                {
                    return false;
                }

                var value = JsonSerializer.Serialize(new
                {
                    round.RoundId,
                    LogOn = DateTimeOffset.UtcNow.ToString("O")
                });

                _ = await _database.StringSetAsync(
                    key: key,
                    value: value,
                    expiry: _expiry,
                    when: When.Always,
                    flags: CommandFlags.None).ConfigureAwait(false);

                return true;
            }
            return true;
        }

        public ValueTask<GameRoundInfo?> GetAsync(
            Guid roundId,
            CancellationToken cancellationToken = default)
            => _gameRound.GetAsync(
                roundId,
                cancellationToken: cancellationToken);

        public ValueTask<GameRoundInfo?> GetAsync(
            Guid gameProviderId,
            string externalRoundId,
            int gameId,
            Guid walletId,
            CancellationToken cancellationToken = default)
            => _gameRound.GetAsync(
                gameProviderId: gameProviderId,
                externalRoundId: externalRoundId,
                gameId: gameId,
                walletId: walletId,
                cancellationToken: cancellationToken);

        public ValueTask<GameRoundDetailInfo?> GetRoundDetailAsync(
            Guid roundId,
            CancellationToken cancellationToken = default)
            => _gameRound.GetRoundDetailAsync(
                roundId: roundId,
                cancellationToken: cancellationToken);

        public async ValueTask<Guid?> GetRoundIdAsync(
            Guid gameProviderId,
            string externalRoundId,
            int gameId,
            Guid walletId,
            CancellationToken cancellationToken = default)
        {
            string key = string.Format(KeyRoundId, gameProviderId, externalRoundId, gameId, walletId).ToLower();

            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var roundId = await _gameRound.GetRoundIdAsync(
                                    gameProviderId: gameProviderId,
                                    externalRoundId: externalRoundId,
                                    gameId: gameId,
                                    walletId: walletId,
                                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!roundId.HasValue)
                {
                    return roundId;
                }

                _ = await _database.StringSetAsync(
                    key: key,
                    value: roundId.Value.ToString(),
                    expiry: _expiry,
                    when: When.Always,
                    flags: CommandFlags.None).ConfigureAwait(false);

                return roundId;
            }

            return Guid.TryParse(await _database.StringGetAsync(key).ConfigureAwait(false), out var tempRoundId)
                ? tempRoundId
                : null;
        }

        public ValueTask<Guid?> NewRoundAsync(
            NewRound source,
            CancellationToken cancellationToken = default)
            => _gameRound.NewRoundAsync(
                source: source,
                cancellationToken: cancellationToken);

        public ValueTask UpdateClientStatusAsync(
            Guid roundId,
            GameRoundClientStatus status,
            CancellationToken cancellationToken = default)
            => _gameRound.UpdateClientStatusAsync(
                roundId: roundId,
                status: status,
                cancellationToken: cancellationToken);

        public ValueTask UpdateRoundClosedAsync(
            UpdateRoundClosed source,
            CancellationToken cancellationToken = default)
            => _gameRound.UpdateRoundClosedAsync(
                source: source,
                cancellationToken: cancellationToken);

        public ValueTask UpdateRoundDetailAsync(
            Guid roundId,
            string additionalData,
            CancellationToken cancellationToken = default)
            => _gameRound.UpdateRoundDetailAsync(
                roundId: roundId,
                additionalData: additionalData,
                cancellationToken: cancellationToken);

        public ValueTask UpdateRoundOpenAsync(
            UpdateRoundOpen source,
            CancellationToken cancellationToken = default)
             => _gameRound.UpdateRoundOpenAsync(
                 source: source,
                cancellationToken: cancellationToken);

        public ValueTask UpdateRoundOpenSettleAsync(
            UpdateRoundOpenSettle source,
            CancellationToken cancellationToken = default)
             => _gameRound.UpdateRoundOpenSettleAsync(
                 source: source,
                cancellationToken: cancellationToken);

        public ValueTask UpdateRoundReopenAsync(
            UpdateRoundReopen source,
            CancellationToken cancellationToken = default)
            => _gameRound.UpdateRoundReopenAsync(
                source: source,
                cancellationToken: cancellationToken);

        public ValueTask UpdateStatusAsync(
            Guid roundId,
            GameRoundStatus status,
            DateTimeOffset? externalClosedOn = null,
            CancellationToken cancellationToken = default)
            => _gameRound.UpdateStatusAsync(
                roundId: roundId,
                status: status,
                externalClosedOn: externalClosedOn,
                cancellationToken: cancellationToken);
    }
}
