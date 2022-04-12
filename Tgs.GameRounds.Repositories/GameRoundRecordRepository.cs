using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Tgs.GameRounds.Core;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories
{
    internal class GameRoundRecordRepository : IGameRoundRecordRepository
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<GameRoundRecordRepository> _logger;
        private readonly ISerialNumberGenerator _serialNumberGenerator;

        public GameRoundRecordRepository(IMongoDatabase database, ILogger<GameRoundRecordRepository> logger, ISerialNumberGenerator serialNumberGenerator)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
        }

        public async ValueTask AddAsync(GameRoundInfo gameRound)
        {
            try
            {
                var collection = await GetCollectionAsync().ConfigureAwait(false);
                var gameCashRecordId = await _serialNumberGenerator.GenerateAsync().ConfigureAwait(false);
                await collection.InsertOneAsync(new GameRoundRecord()
                {
                    GameRoundRecordId = gameCashRecordId,
                    RoundId = gameRound.RoundId,
                    GameProviderId = gameRound.GameProviderId,
                    GameProviderConfigurationId = gameRound.GameProviderConfigurationId,
                    ExternalRoundId = gameRound.ExternalRoundId,
                    BrandId = gameRound.BrandId,
                    PlayerCode = gameRound.PlayerCode,
                    WalletId = gameRound.WalletId,
                    GameId = gameRound.GameId,
                    CurrencyCode = gameRound.CurrencyCode,
                    Status = (int)gameRound.Status,
                    CreatedOn = gameRound.CreatedOn,
                    ClosedOn = gameRound.ClosedOn,
                    UpdatedOn = gameRound.UpdatedOn,
                    ExternalCreatedOn = gameRound.ExternalCreatedOn,
                    ExternalClosedOn = gameRound.ExternalClosedOn,
                    Bet = string.Format("{0:0.000000}", gameRound.BetAmount.GetValueOrDefault(0)),
                    ValidBet = string.Format("{0:0.000000}", gameRound.ValidBetAmount.GetValueOrDefault(0)),
                    Settle = string.Format("{0:0.000000}", gameRound.SettleAmount.GetValueOrDefault(0)),
                    Commission = string.Format("{0:0.000000}", gameRound.Commission.GetValueOrDefault(0)),
                    Turnover = string.Format("{0:0.000000}", gameRound.Turnover.GetValueOrDefault(0)),
                    WinLoss = string.Format("{0:0.000000}", gameRound.WinLoss.GetValueOrDefault(0)),
                    TieBetAdjustment = string.Format("{0:0.000000}", gameRound.TieBetAdjustment.GetValueOrDefault(0)),
                    ClientStatus = (int)gameRound.ClientStatus,
                    ReopenTurnoverAdjustment = string.Format("{0:0.000000}", gameRound.ReopenTurnoverAdjustment.GetValueOrDefault(0)),
                    ReopenedOn = gameRound.ReopenedOn,
                    RecordOn = DateTimeOffset.UtcNow
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, "LogOn:{DateTime.Now.ToString(\"o\")} ErrorMessage: {ex.Message}", DateTime.Now.ToString("o"), ex.Message);
            }
        }

        private static Task CreatedOnIndexAsync(IMongoCollection<GameRoundRecord> collection)
        {
            var index = Builders<GameRoundRecord>.IndexKeys
                .Ascending(m => m.RecordOn);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"ix_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}_record_on"
            };
            var createIndexModel = new CreateIndexModel<GameRoundRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(createIndexModel);
        }

        private static Task PrimaryKeyIndexAsync(IMongoCollection<GameRoundRecord> collection)
        {
            var index = Builders<GameRoundRecord>.IndexKeys
               .Ascending(m => m.GameRoundRecordId);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"pk_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}"
            };
            var createIndexModel = new CreateIndexModel<GameRoundRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(createIndexModel);
        }

        private static Task RoundIdIndexAsync(IMongoCollection<GameRoundRecord> collection)
        {
            var index = Builders<GameRoundRecord>.IndexKeys
               .Ascending(m => m.RoundId);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"ix_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}_round_id"
            };
            var createIndexModel = new CreateIndexModel<GameRoundRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(createIndexModel);
        }

        private async ValueTask<bool> CollectionExistsAsync(string collectionName)
            => await (await _database.ListCollectionsAsync(new ListCollectionsOptions { Filter = new BsonDocument("name", collectionName) }).ConfigureAwait(false)).AnyAsync().ConfigureAwait(false);

        private async Task<IMongoCollection<GameRoundRecord>> GetCollectionAsync()
        {
            var collection = _database.GetCollection<GameRoundRecord>(typeof(GameRoundRecord).Name);
            if (collection.CollectionNamespace == null)
            {
                return collection;
            }

            if (!await CollectionExistsAsync(collection.CollectionNamespace.CollectionName.ToString()).ConfigureAwait(false))
            {
                await PrimaryKeyIndexAsync(collection).ConfigureAwait(false);
                await CreatedOnIndexAsync(collection).ConfigureAwait(false);
                await RoundIdIndexAsync(collection).ConfigureAwait(false);
            }

            return collection;
        }
    }
}
