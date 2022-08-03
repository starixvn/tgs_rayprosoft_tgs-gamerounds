using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories
{
    internal class GameRoundRecordRepository : IGameRoundRecord
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
            AddGameRoundRecord add,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var collection = await GetCollectionAsync(
                    gameProviderCode: add.GameProviderCode,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                var gameCashRecordId = await _serialNumberGenerator.GenerateAsync(
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                var document = new GameRoundRecord()
                {
                    GameRoundRecordId = gameCashRecordId,
                    RoundId = add.RoundId,
                    ExternalRoundId = add.ExternalRoundId,
                    ExternalGameId = add.ExternalGameId,
                    GameProviderId = add.GameProviderId,
                    GameProviderCode = add.GameProviderCode,
                    GameProviderConfigurationId = add.GameProviderConfigurationId,
                    GameId = add.GameId,
                    BrandId = add.BrandId,
                    PlayerCode = add.PlayerCode,
                    WalletId = add.WalletId,
                    CurrencyCode = add.CurrencyCode,
                    Status = (int)add.Status,
                    ClientStatus = (int)add.ClientStatus,
                    CreatedOn = add.CreatedOn,
                    ClosedOn = add.ClosedOn,
                    UpdatedOn = add.UpdatedOn,
                    ExternalCreatedOn = add.ExternalCreatedOn,
                    ExternalClosedOn = add.ExternalClosedOn,
                    Bet = add.Bet,
                    ValidBet = add.ValidBet,
                    Settle = add.Settle,
                    Commission = add.Commission,
                    Turnover = add.Turnover,
                    CompanyWinLoss = add.CompanyWinLoss,
                    TieBetAdjustment = add.TieBetAdjustment,
                    ReopenTurnoverAdjustment = add.ReopenTurnoverAdjustment,
                    ReopenedOn = add.ReopenedOn,
                    RecordOn = DateTime.UtcNow
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

        private static Task ExternalRoundId_ExternalGameId_PlayerCode_IndexAsync(
            IMongoCollection<GameRoundRecord> collection,
            CancellationToken cancellationToken = default)
        {
            var index = Builders<GameRoundRecord>.IndexKeys
               .Ascending(m => m.ExternalRoundId)
               .Ascending(m => m.ExternalGameId)
               .Ascending(m => m.PlayerCode);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"ix_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}_external_round_id_external_game_id_player_code"
            };
            var createIndexModel = new CreateIndexModel<GameRoundRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(
                model: createIndexModel,
                cancellationToken: cancellationToken);
        }

        private static Task ExternalRoundId_ExternalGameId_WalletId_IndexAsync(
            IMongoCollection<GameRoundRecord> collection,
            CancellationToken cancellationToken = default)
        {
            var index = Builders<GameRoundRecord>.IndexKeys
               .Ascending(m => m.ExternalRoundId)
               .Ascending(m => m.ExternalGameId)
               .Ascending(m => m.WalletId);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"ix_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}_external_round_id_external_game_id_wallet_id"
            };
            var createIndexModel = new CreateIndexModel<GameRoundRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(
                model: createIndexModel,
                cancellationToken: cancellationToken);
        }

        private static Task ExternalRoundId_GameId_PlayerCode_IndexAsync(
            IMongoCollection<GameRoundRecord> collection,
            CancellationToken cancellationToken = default)
        {
            var index = Builders<GameRoundRecord>.IndexKeys
               .Ascending(m => m.ExternalRoundId)
               .Ascending(m => m.GameId)
               .Ascending(m => m.PlayerCode);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"ix_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}_external_round_id_game_id_player_code"
            };
            var createIndexModel = new CreateIndexModel<GameRoundRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(
                model: createIndexModel,
                cancellationToken: cancellationToken);
        }

        private static Task ExternalRoundId_GameId_WalletId_IndexAsync(
            IMongoCollection<GameRoundRecord> collection,
            CancellationToken cancellationToken = default)
        {
            var index = Builders<GameRoundRecord>.IndexKeys
               .Ascending(m => m.ExternalRoundId)
               .Ascending(m => m.GameId)
               .Ascending(m => m.WalletId);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"ix_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}_external_round_id_game_id_wallet_id"
            };
            var createIndexModel = new CreateIndexModel<GameRoundRecord>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(
                model: createIndexModel,
                cancellationToken: cancellationToken);
        }

        private static Task PrimaryKey_IndexAsync(
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

        private static Task RecordOn_IndexAsync(
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

        private static Task RoundId_IndexAsync(
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

        private async Task<IMongoCollection<GameRoundRecord>> GetCollectionAsync(string gameProviderCode, CancellationToken cancellationToken = default)
        {
            var collectionName = $"{gameProviderCode.ToUpper()}{typeof(GameRoundRecord).Name}";
            var collection = _database.GetCollection<GameRoundRecord>(collectionName);
            if (collection.CollectionNamespace == null)
            {
                return collection;
            }

            if (!await CollectionExistsAsync(
                collectionName: collection.CollectionNamespace.CollectionName.ToString(),
                cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                await PrimaryKey_IndexAsync(
                    collection: collection,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                await RecordOn_IndexAsync(
                    collection: collection,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                await RoundId_IndexAsync(
                    collection: collection,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                await ExternalRoundId_ExternalGameId_WalletId_IndexAsync(
                    collection: collection,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                await ExternalRoundId_GameId_WalletId_IndexAsync(
                    collection: collection,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                await ExternalRoundId_ExternalGameId_PlayerCode_IndexAsync(
                    collection: collection,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                await ExternalRoundId_GameId_PlayerCode_IndexAsync(
                    collection: collection,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            return collection;
        }
    }
}
