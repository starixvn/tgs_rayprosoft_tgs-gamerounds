using System.Text.Json;
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
                    Bet = gameRound.BetAmount,
                    ValidBet = gameRound.ValidBetAmount,
                    Settle = gameRound.SettleAmount,
                    Commission = gameRound.Commission,
                    Turnover = gameRound.Turnover,
                    WinLoss = gameRound.WinLoss,
                    TieBetAdjustment = gameRound.TieBetAdjustment,
                    ClientStatus = (int)gameRound.ClientStatus,
                    ReopenTurnoverAdjustment = gameRound.ReopenTurnoverAdjustment,
                    ReopenedOn = gameRound.ReopenedOn,
                    AdditionalData = gameRound.AdditionalData,
                    RecordOn = DateTimeOffset.UtcNow
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                   new EventId(10000, nameof(GameRoundRepository)),
                   gameRound,
                   ex,
                   (state, ex) => JsonSerializer.Serialize(new
                   {
                       Data = gameRound,
                       Exception = ex!,
                       Message = "添加紀錄出現例外",
                       LogOn = DateTimeOffset.UtcNow.ToString("o")
                   }));
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
