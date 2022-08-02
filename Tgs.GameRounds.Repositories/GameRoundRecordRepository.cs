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

        public GameRoundRecordRepository(
            IMongoDatabase database,
            ILogger<GameRoundRecordRepository> logger,
            ISerialNumberGenerator serialNumberGenerator)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
        }

        public async ValueTask AddAsync(
            GameRoundInfo gameRound,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var collection = await GetCollectionAsync(
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                var gameCashRecordId = await _serialNumberGenerator.GenerateAsync(
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                var document = new GameRoundRecord()
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
                    Bet = gameRound.BetAmount.GetValueOrDefault(0),
                    ValidBet = gameRound.ValidBetAmount.GetValueOrDefault(0),
                    Settle = gameRound.SettleAmount.GetValueOrDefault(0),
                    Commission = gameRound.Commission.GetValueOrDefault(0),
                    Turnover = gameRound.Turnover.GetValueOrDefault(0),
                    CompanyWinLoss = gameRound.CompanyWinLoss.GetValueOrDefault(0),
                    TieBetAdjustment = gameRound.TieBetAdjustment.GetValueOrDefault(0),
                    ClientStatus = (int)gameRound.ClientStatus,
                    ReopenTurnoverAdjustment = gameRound.ReopenTurnoverAdjustment.GetValueOrDefault(0),
                    RecordOn = DateTimeOffset.UtcNow
                };

                await collection.InsertOneAsync(
                    document: document,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, "LogOn:{DateTime.Now.ToString(\"o\")} ErrorMessage: Failed to build GameRoundRecord.", DateTime.Now.ToString("o"));
            }
        }

        private static Task CreatedOnIndexAsync(
            IMongoCollection<GameRoundRecord> collection,
            CancellationToken cancellationToken = default)
        {
            var index = Builders<GameRoundRecord>.IndexKeys
                .Ascending(m => m.RecordOn);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"ix_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}_record_on"
            };
            var createIndexModel = new CreateIndexModel<GameRoundRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(
                model: createIndexModel,
                cancellationToken: cancellationToken);
        }

        private static Task PrimaryKeyIndexAsync(
            IMongoCollection<GameRoundRecord> collection,
            CancellationToken cancellationToken = default)
        {
            var index = Builders<GameRoundRecord>.IndexKeys
               .Ascending(m => m.GameRoundRecordId);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"pk_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}"
            };
            var createIndexModel = new CreateIndexModel<GameRoundRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(
                model: createIndexModel,
                cancellationToken: cancellationToken);
        }

        private static Task RoundIdIndexAsync(
            IMongoCollection<GameRoundRecord> collection,
            CancellationToken cancellationToken = default)
        {
            var index = Builders<GameRoundRecord>.IndexKeys
               .Ascending(m => m.RoundId);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"ix_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}_round_id"
            };
            var createIndexModel = new CreateIndexModel<GameRoundRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(
                model: createIndexModel,
                cancellationToken: cancellationToken);
        }

        private async ValueTask<bool> CollectionExistsAsync(
            string collectionName,
            CancellationToken cancellationToken = default)
        {
            var options = new ListCollectionsOptions { Filter = new BsonDocument("name", collectionName) };

            var cursor = await _database.ListCollectionsAsync(
                options: options,
                 cancellationToken: cancellationToken).ConfigureAwait(false);

            return await cursor.AnyAsync(
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private async Task<IMongoCollection<GameRoundRecord>> GetCollectionAsync(CancellationToken cancellationToken = default)
        {
            var collection = _database.GetCollection<GameRoundRecord>(typeof(GameRoundRecord).Name);
            if (collection.CollectionNamespace == null)
            {
                return collection;
            }

            if (!await CollectionExistsAsync(
                collectionName: collection.CollectionNamespace.CollectionName.ToString(),
                cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                await PrimaryKeyIndexAsync(
                    collection: collection,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                await CreatedOnIndexAsync(
                    collection: collection,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                await RoundIdIndexAsync(
                    collection: collection,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            return collection;
        }
    }
}
