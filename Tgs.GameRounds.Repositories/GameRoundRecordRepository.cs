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
			Name = $"ix__external_round_id_external_game_id_player_code"
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
			Name = $"ix_external_round_id_external_game_id_wallet_id"
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
			Name = $"ix_external_round_id_game_id_player_code"
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
			Name = $"ix_external_round_id_game_id_wallet_id"
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

	private static Task ReceiveOn_IndexAsync(
		IMongoCollection<GameRoundRecord> collection,
		CancellationToken cancellationToken = default)
	{
		var index = Builders<GameRoundRecord>.IndexKeys
			.Ascending(m => m.ReceiveOn);

		var createIndexOptions = new CreateIndexOptions
		{
			Name = $"ix_receive_on"
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
			Name = $"ix_record_on"
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

	private static Task SendOn_IndexAsync(
		IMongoCollection<GameRoundRecord> collection,
		CancellationToken cancellationToken = default)
	{
		var index = Builders<GameRoundRecord>.IndexKeys
			.Ascending(m => m.SendOn);

		var createIndexOptions = new CreateIndexOptions
		{
			Name = $"ix_send_on"
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
			await PrimaryKey_IndexAsync(
				collection: collection,
				cancellationToken: cancellationToken).ConfigureAwait(false);

			await RecordOn_IndexAsync(
				collection: collection,
				cancellationToken: cancellationToken).ConfigureAwait(false);

			await ReceiveOn_IndexAsync(
				collection: collection,
				cancellationToken: cancellationToken).ConfigureAwait(false);

			await SendOn_IndexAsync(
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
