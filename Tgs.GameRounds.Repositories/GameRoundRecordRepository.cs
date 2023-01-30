using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using MongoDB.Bson;
using MongoDB.Driver;

using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories;

internal class GameRoundRecordRepository : IGameRoundRecord
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<GameRoundRecordRepository> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    public GameRoundRecordRepository(
        IMongoDatabase database,
        IMemoryCache memoryCache,
        ILogger<GameRoundRecordRepository> logger,
        ISerialNumberGenerator serialNumberGenerator)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
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
                CreatedOn = add.CreatedOn.UtcDateTime,
                ClosedOn = add.ClosedOn.HasValue ? add.ClosedOn.Value.UtcDateTime : null,
                UpdatedOn = add.UpdatedOn.UtcDateTime,
                ExternalCreatedOn = add.ExternalCreatedOn.HasValue ? add.ExternalCreatedOn.Value.UtcDateTime : null,
                ExternalClosedOn = add.ExternalClosedOn.HasValue ? add.ExternalClosedOn.Value.UtcDateTime : null,
                Bet = add.Bet,
                ValidBet = add.ValidBet,
                Settle = add.Settle,
                Commission = add.Commission,
                Turnover = add.Turnover,
                CompanyWinLoss = add.CompanyWinLoss,
                TieBetAdjustment = add.TieBetAdjustment,
                ReopenTurnoverAdjustment = add.ReopenTurnoverAdjustment,
                ReopenedOn = add.ReopenedOn.HasValue ? add.ReopenedOn.Value.UtcDateTime : null,
                SendOn = add.SendOn.UtcDateTime,
                ReceiveOn = add.ReceiveOn.UtcDateTime,
                RecordOn = DateTime.UtcNow,
                ExpireOn = DateTime.UtcNow.AddDays(+7)
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

    private static Task ExpireOn_IndexAsync(
        IMongoCollection<GameRoundRecord> collection,
        CancellationToken cancellationToken = default)
    {
        var index = Builders<GameRoundRecord>.IndexKeys
            .Ascending(m => m.ExpireOn);

        var createIndexOptions = new CreateIndexOptions
        {
            ExpireAfter = TimeSpan.FromSeconds(1),
            Name = $"ix_expire_on"
        };
        var createIndexModel = new CreateIndexModel<GameRoundRecord>(index, createIndexOptions);

        return collection.Indexes.CreateOneAsync(
            model: createIndexModel,
            cancellationToken: cancellationToken);
    }

    private static Task ExternalRoundId_IndexAsync(
        IMongoCollection<GameRoundRecord> collection,
        CancellationToken cancellationToken = default)
    {
        var index = Builders<GameRoundRecord>.IndexKeys
            .Ascending(m => m.ExternalRoundId);

        var createIndexOptions = new CreateIndexOptions
        {
            Name = $"ix_external_round_id"
        };
        var createIndexModel = new CreateIndexModel<GameRoundRecord>(index, createIndexOptions);

        return collection.Indexes.CreateOneAsync(
            model: createIndexModel,
            cancellationToken: cancellationToken);
    }

    private static Task PlayerCode_IndexAsync(
        IMongoCollection<GameRoundRecord> collection,
        CancellationToken cancellationToken = default)
    {
        var index = Builders<GameRoundRecord>.IndexKeys
            .Ascending(m => m.PlayerCode);

        var createIndexOptions = new CreateIndexOptions
        {
            Name = $"ix_player_code"
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
            Name = $"ix_round_id"
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
        var key = $"mongodb:{collectionName}";

        if (!_memoryCache.TryGetValue(key, out bool isExist))
        {
            var options = new ListCollectionsOptions { Filter = new BsonDocument("name", collectionName) };

            var cursor = await _database
                .ListCollectionsAsync(
                    options: options,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            isExist = await cursor
                .AnyAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (isExist)
            {
                _ = _memoryCache.Set(key, isExist, TimeSpan.FromMinutes(3));
            }
        }

        return isExist;
    }

    private async Task<IMongoCollection<GameRoundRecord>> GetCollectionAsync(string gameProviderCode, CancellationToken cancellationToken = default)
    {
        var collectionName = $"{gameProviderCode.ToUpper()}_game_round_records".ToLower();
        var collection = _database.GetCollection<GameRoundRecord>(collectionName);
        if (collection.CollectionNamespace == null)
        {
            return collection;
        }

        if (!await CollectionExistsAsync(
            collectionName: collection.CollectionNamespace.CollectionName.ToString(),
            cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            await RoundId_IndexAsync(
                collection: collection,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await ExternalRoundId_IndexAsync(
                collection: collection,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await PlayerCode_IndexAsync(
                collection: collection,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await ExpireOn_IndexAsync(
                collection: collection,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return collection;
    }
}
