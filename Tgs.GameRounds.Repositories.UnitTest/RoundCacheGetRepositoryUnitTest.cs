using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Core;
using StackExchange.Redis;
using System.Text.Json.Serialization;
using System.Text.Json;
using Tgs.GameRounds.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tgs.GameRounds.Repositories.UnitTest;

public class RoundCacheGetRepositoryUnitTest
{
	private static readonly JsonSerializerOptions _SerializerOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};

	[Fact]
	public async Task Get_This_Data_Is_Retrieved_From_The_Database()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheGetRepository>.Instance;

		var id = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;

		var key = string.Concat(typeof(RoundCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "get", ":", ":", "id", ":", id);

		_ = fakeDatabase.KeyExistsAsync(
			key: Arg.Is<RedisKey>(key))
			.Returns(false);

		var info = new RoundInfo(
			Id: id,
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			ExternalRoundId: DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.Now,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository.GetAsync(
			externalCreatedOn: Arg.Is(externalCreatedOn),
			id: Arg.Is(id),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<RoundInfo?>(info));

		var sut = new RoundCacheGetRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		var actual = await sut.GetAsync(externalCreatedOn, id);

		Assert.NotNull(actual);
	}

	[Fact]
	public async Task Get_This_Data_Is_Retrieved_From_Cache_Database()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheGetRepository>.Instance;

		var id = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;

		var key = string.Concat(typeof(RoundCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "get", ":", ":", "id", ":", id);

		_ = fakeDatabase.KeyExistsAsync(
			key: Arg.Is<RedisKey>(key))
			.Returns(true);

		var info = new RoundInfo(
			Id: id,
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			ExternalRoundId: DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.Now,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeDatabase.StringGetAsync(
			key: Arg.Is<RedisKey>(key))
			.Returns(JsonSerializer.Serialize(info, _SerializerOptions));

		var sut = new RoundCacheGetRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		var actual = await sut.GetAsync(externalCreatedOn, id);

		Assert.NotNull(actual);

		_ = fakeRoundRepository
			.Received(0)
			.GetAsync(
			externalCreatedOn: Arg.Is(externalCreatedOn),
			id: Arg.Is(id),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Get_This_Data_Is_Empty()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheGetRepository>.Instance;

		var id = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;

		var key = string.Concat(typeof(RoundCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "get", ":", ":", "id", ":", id);

		_ = fakeDatabase.KeyExistsAsync(
			key: Arg.Is<RedisKey>(key))
			.Returns(false
			);

		_ = fakeRoundRepository.GetAsync(
			externalCreatedOn: Arg.Is(externalCreatedOn),
			id: Arg.Is(id),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<RoundInfo?>(null));

		var sut = new RoundCacheGetRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		var actual = await sut.GetAsync(externalCreatedOn, id);

		Assert.Null(actual);
	}

	[Fact]
	public async Task Get_An_Exception_Occurred_While_Accessing_This_Data_From_The_Cache_Database()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheGetRepository>.Instance;

		var id = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;

		var key = string.Concat(typeof(RoundCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "get", ":", ":", "id", ":", id);

		_ = fakeDatabase.KeyExistsAsync(
			key: Arg.Is<RedisKey>(key))
			.Returns(Task.FromException<bool>(new RedisException(nameof(RedisException))));

		var info = new RoundInfo(
			Id: id,
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			ExternalRoundId: DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.Now,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository.GetAsync(
			externalCreatedOn: Arg.Is(externalCreatedOn),
			id: Arg.Is(id),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<RoundInfo?>(info));

		var sut = new RoundCacheGetRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		var actual = await sut.GetAsync(externalCreatedOn, id);

		Assert.NotNull(actual);
	}

	[Fact]
	public async Task Get_A_Timeout_Exception_Occurred_When_Retrieving_This_Data_From_The_Cache_Database()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheGetRepository>.Instance;

		var id = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;

		var key = string.Concat(typeof(RoundCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "get", ":", ":", "id", ":", id);

		_ = fakeDatabase.KeyExistsAsync(
			key: Arg.Is<RedisKey>(key))
			.Returns(Task.FromException<bool>(new RedisTimeoutException(nameof(RedisTimeoutException), CommandStatus.Unknown)));

		var info = new RoundInfo(
			Id: id,
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			ExternalRoundId: DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.Now,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository.GetAsync(
			externalCreatedOn: Arg.Is(externalCreatedOn),
			id: Arg.Is(id),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<RoundInfo?>(info));

		var sut = new RoundCacheGetRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		var actual = await sut.GetAsync(externalCreatedOn, id);
		Assert.NotNull(actual);
	}
	[Fact]
	public async Task Get_A_Command_Exception_Occurred_While_Accessing_This_Data_From_The_Cache_Database()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheGetRepository>.Instance;

		var id = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;

		var key = string.Concat(typeof(RoundCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "get", ":", ":", "id", ":", id);

		_ = fakeDatabase.KeyExistsAsync(
			key: Arg.Is<RedisKey>(key))
			.Returns(Task.FromException<bool>(new RedisCommandException(nameof(RedisCommandException))));

		var info = new RoundInfo(
			Id: id,
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			ExternalRoundId: DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.Now,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository.GetAsync(
			externalCreatedOn: Arg.Is(externalCreatedOn),
			id: Arg.Is(id),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<RoundInfo?>(info));

		var sut = new RoundCacheGetRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		var actual = await sut.GetAsync(externalCreatedOn, id);

		Assert.NotNull(actual);
	}

	[Fact]
	public async Task Add()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheGetRepository>.Instance;

		var id = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var source = new RoundAdd(
			Id: id,
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Win,
			ExternalRoundId: DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			CreatedOn: DateTimeOffset.UtcNow);

		var sut = new RoundCacheGetRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		await sut.AddAsync(source);

		_ = fakeRoundRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is(source),
			cancellationToken: Arg.Any<CancellationToken>());

		var key = string.Concat(typeof(RoundCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "get", ":", ":", "id", ":", id);

		_ = fakeDatabase
			.Received(0)
			.KeyDeleteAsync(
			key: Arg.Is<RedisKey>(key),
			flags: Arg.Is(CommandFlags.FireAndForget));
	}

	[Fact]
	public async Task Closed()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheGetRepository>.Instance;

		var id = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var source = new RoundClosed(
			ClientStatus: Common.Enums.RoundClientStatus.Win,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Turnover: 10.0m,
			Commission: 0.0m,
			PlayerWinLoss: 10.0m);

		var sut = new RoundCacheGetRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		await sut.ClosedAsync(externalCreatedOn, id, source);

		_ = fakeRoundRepository
			.Received(1)
			.ClosedAsync(
			externalCreatedOn: Arg.Is(externalCreatedOn),
			id: Arg.Is(id),
			source: Arg.Is(source),
			cancellationToken: Arg.Any<CancellationToken>());

		var key = string.Concat(typeof(RoundCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "get", ":", ":", "id", ":", id);

		_ = fakeDatabase
			.Received(1)
			.KeyDeleteAsync(
			key: Arg.Is<RedisKey>(key),
			flags: Arg.Is(CommandFlags.FireAndForget));
	}

	[Fact]
	public async Task Open()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheGetRepository>.Instance;

		var id = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var source = new RoundOpen(
			Bet: 10.0m,
			ValidBet: 10.0m,
			Turnover: 10.0m);

		var sut = new RoundCacheGetRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		await sut.OpenAsync(externalCreatedOn, id, source);

		_ = fakeRoundRepository
			.Received(1)
			.OpenAsync(
			externalCreatedOn: Arg.Is(externalCreatedOn),
			id: Arg.Is(id),
			source: Arg.Is(source),
			cancellationToken: Arg.Any<CancellationToken>());

		var key = string.Concat(typeof(RoundCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "get", ":", ":", "id", ":", id);

		_ = fakeDatabase
			.Received(1)
			.KeyDeleteAsync(
			key: Arg.Is<RedisKey>(key),
			flags: Arg.Is(CommandFlags.FireAndForget));
	}

	[Fact]
	public async Task Reopen()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheGetRepository>.Instance;

		var id = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var source = new RoundReopen(
			ClientStatus: Common.Enums.RoundClientStatus.Win,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Turnover: 10.0m,
			Commission: 0.0m,
			PlayerWinLoss: 10.0m);

		var sut = new RoundCacheGetRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		await sut.ReopenAsync(externalCreatedOn, id, source);

		_ = fakeRoundRepository
			.Received(1)
			.ReopenAsync(
			externalCreatedOn: Arg.Is(externalCreatedOn),
			id: Arg.Is(id),
			source: Arg.Is(source),
			cancellationToken: Arg.Any<CancellationToken>());

		var key = string.Concat(typeof(RoundCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "get", ":", ":", "id", ":", id);

		_ = fakeDatabase
			.Received(1)
			.KeyDeleteAsync(
			key: Arg.Is<RedisKey>(key),
			flags: Arg.Is(CommandFlags.FireAndForget));
	}
}
