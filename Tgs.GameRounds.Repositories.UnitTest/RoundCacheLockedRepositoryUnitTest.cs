using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using StackExchange.Redis;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Repositories.UnitTest;

public class RoundCacheLockedRepositoryUnitTest
{
	[Fact]
	public async Task Add_No_Lock()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheLockedRepository>.Instance;

		var source = new RoundAdd(
			Id: Guid.NewGuid(),
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
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			CreatedOn: DateTimeOffset.UtcNow);

		var key = string.Concat(typeof(RoundCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "locked", ":", source.GameProviderId, ":", source.GameId, ":", source.WalletId, ":", source.ExternalRoundId);

		_ = fakeDatabase.StringSetAsync(
			key: Arg.Is<RedisKey>(key),
			value: Arg.Any<RedisValue>(),
			expiry: Arg.Is<TimeSpan?>(fakeTimeSpan),
			when: Arg.Is(When.NotExists))
			.Returns(true);

		var sut = new RoundCacheLockedRepository(
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
	}

	[Fact]
	public async Task Add_Is_Lock()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheLockedRepository>.Instance;

		var source = new RoundAdd(
			Id: Guid.NewGuid(),
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
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Turnover: 10.0m,
			Commission: 0.0m,
			PlayerWinLoss: 10.0m,
			CreatedOn: DateTimeOffset.UtcNow);

		var key = string.Concat(typeof(RoundCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "locked", ":", source.GameProviderId, ":", source.GameId, ":", source.WalletId, ":", source.ExternalRoundId);

		_ = fakeDatabase.StringSetAsync(
			key: Arg.Is<RedisKey>(key),
			value: Arg.Any<RedisValue>(),
			expiry: Arg.Is<TimeSpan?>(fakeTimeSpan),
			when: Arg.Is(When.NotExists))
			.Returns(false);

		var sut = new RoundCacheLockedRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		await sut.AddAsync(source);

		_ = fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Is(source),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Add_Is_Exception()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheLockedRepository>.Instance;

		var source = new RoundAdd(
			Id: Guid.NewGuid(),
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
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			CreatedOn: DateTimeOffset.UtcNow);

		var key = string.Concat(typeof(RoundCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "locked", ":", source.GameProviderId, ":", source.GameId, ":", source.WalletId, ":", source.ExternalRoundId);

		_ = fakeDatabase.StringSetAsync(
			key: Arg.Is<RedisKey>(key),
			value: Arg.Any<RedisValue>(),
			expiry: Arg.Is<TimeSpan?>(fakeTimeSpan),
			when: Arg.Is(When.NotExists))
			.Returns(true);

		_ = fakeRoundRepository
			.AddAsync(
			source: Arg.Is(source),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromException(new Exception()));

		var sut = new RoundCacheLockedRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.AddAsync(source).AsTask());

		_ = fakeDatabase
			.Received(1)
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
		var fakeLogger = NullLogger<RoundCacheLockedRepository>.Instance;

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

		var sut = new RoundCacheLockedRepository(
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
	}

	[Fact]
	public async Task Get()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheLockedRepository>.Instance;

		var id = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.Now;

		var sut = new RoundCacheLockedRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		_ = await sut.GetAsync(externalCreatedOn, id);

		_ = fakeRoundRepository
			.Received(1)
			.GetAsync(
			externalCreatedOn: Arg.Is(externalCreatedOn),
			id: Arg.Is(id),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Get_Id()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheLockedRepository>.Instance;

		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = DateTimeOffset.Now;

		var sut = new RoundCacheLockedRepository(
			fakeRoundRepository,
			fakeDatabase,
			fakeTimeSpan,
			fakeLogger);

		_ = await sut.GetIdAsync(externalCreatedOn, gameProviderId, gameId, walletId, externalRoundId);

		_ = fakeRoundRepository
			.Received(1)
			.GetIdAsync(
			externalCreatedOn: Arg.Is(externalCreatedOn),
			gameProviderId: Arg.Is(gameProviderId),
			gameId: Arg.Is(gameId),
			walletId: Arg.Is(walletId),
			externalRoundId: Arg.Is(externalRoundId),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Open()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheLockedRepository>.Instance;

		var id = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var source = new RoundOpen(
			Bet: 10.0m,
			ValidBet: 10.0m,
			Turnover: 10.0m);

		var sut = new RoundCacheLockedRepository(
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
	}

	[Fact]
	public async Task Reopen()
	{
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeDatabase = Substitute.For<IDatabase>();
		var fakeTimeSpan = TimeSpan.FromMinutes(1);
		var fakeLogger = NullLogger<RoundCacheLockedRepository>.Instance;

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

		var sut = new RoundCacheLockedRepository(
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
	}
}
