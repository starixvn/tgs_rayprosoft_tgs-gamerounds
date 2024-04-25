using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Core.UnitTest.ApplicationServices;

public class RoundCommandGetServiceUnitTest
{
	[Fact]
	public async Task Add_Round_Status_Is_Open()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalRoundId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var status = Common.Enums.RoundStatus.Open;
		var clientStatus = Common.Enums.RoundClientStatus.Open;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			Status: status,
			ClientStatus: clientStatus,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		_ = fakeRoundRepository.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalRoundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(null));

		var roundId = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(roundId);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is(new Models.RoundAdd(
				Id: roundId,
				GameProviderId: query.GameProviderId,
				GameProviderConfigurationId: query.GameProviderConfigurationId,
				GameId: query.GameId,
				BrandId: query.BrandId,
				PlayerCode: query.PlayerCode,
				WalletId: query.WalletId,
				CurrencyCode: query.CurrencyCode,
				Status: Common.Enums.RoundStatus.Open,
				ClientStatus: Common.Enums.RoundClientStatus.Open,
				ExternalRoundId: query.ExternalRoundId,
				ExternalCreatedOn: externalCreatedOn,
				ExternalClosedOn: null,
				Bet: query.Bet,
				ValidBet: query.ValidBet,
				Settle: 0.0m,
				Commission: 0.0m,
				Turnover: 0.0m,
				PlayerWinLoss: 0.0m)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundOpen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundClosed>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundReopen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Add_Round_Status_Is_Closed()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalRoundId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalClosedOn = DateTimeOffset.UtcNow;
		var status = Common.Enums.RoundStatus.Closed;
		var clientStatus = Common.Enums.RoundClientStatus.Win;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			Status: status,
			ClientStatus: clientStatus,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		_ = fakeRoundRepository.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalRoundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(null));

		var roundId = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(roundId);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is(new Models.RoundAdd(
				Id: roundId,
				GameProviderId: query.GameProviderId,
				GameProviderConfigurationId: query.GameProviderConfigurationId,
				GameId: query.GameId,
				BrandId: query.BrandId,
				PlayerCode: query.PlayerCode,
				WalletId: query.WalletId,
				CurrencyCode: query.CurrencyCode,
				Status: Common.Enums.RoundStatus.Closed,
				ClientStatus: Common.Enums.RoundClientStatus.Win,
				ExternalRoundId: query.ExternalRoundId,
				ExternalCreatedOn: externalCreatedOn,
				ExternalClosedOn: externalClosedOn,
				Bet: 10.0m,
				ValidBet: 10.0m,
				Settle: 20.0m,
				Commission: 0.0m,
				Turnover: 10.0m,
				PlayerWinLoss: 10.0m)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundOpen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundClosed>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundReopen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Closed()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalRoundId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalClosedOn = DateTimeOffset.UtcNow;
		var status = Common.Enums.RoundStatus.Closed;
		var clientStatus = Common.Enums.RoundClientStatus.Win;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			Status: status,
			ClientStatus: clientStatus,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository
			.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalRoundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfo(
			Id: roundId,
			GameProviderId: query.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: query.GameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: query.PlayerCode,
			WalletId: query.WalletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Open,
			ClientStatus: Common.Enums.RoundClientStatus.Open,
			ExternalRoundId: query.ExternalRoundId,
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			PlayerWinLoss: 0.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.UnixEpoch,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository
			.GetAsync(
			id: Arg.Is(roundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAdd>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundOpen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Is(new Models.RoundClosed(
				ClientStatus: Common.Enums.RoundClientStatus.Win,
				ExternalClosedOn: externalClosedOn,
				Bet: query.Bet,
				ValidBet: query.ValidBet,
				Settle: query.Settle,
				Commission: query.Commission,
				Turnover: query.Turnover,
				PlayerWinLoss: query.PlayerWinLoss)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundReopen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Closed_Get_DataBase_RoundStatus_Is_Closed()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalRoundId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalClosedOn = DateTimeOffset.UtcNow;
		var status = Common.Enums.RoundStatus.Closed;
		var clientStatus = Common.Enums.RoundClientStatus.Win;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			Status: status,
			ClientStatus: clientStatus,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository
			.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalRoundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfo(
			Id: roundId,
			GameProviderId: query.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: query.GameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: query.PlayerCode,
			WalletId: query.WalletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Win,
			ExternalRoundId: query.ExternalRoundId,
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.Now,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository
			.GetAsync(
			id: Arg.Is(roundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAdd>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundOpen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundClosed>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundReopen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Open()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalRoundId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var status = Common.Enums.RoundStatus.Open;
		var clientStatus = Common.Enums.RoundClientStatus.Open;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			Status: status,
			ClientStatus: clientStatus,
			Bet: 20.0m,
			ValidBet: 20.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository
			.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalRoundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfo(
			Id: roundId,
			GameProviderId: query.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: query.GameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: query.PlayerCode,
			WalletId: query.WalletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Open,
			ClientStatus: Common.Enums.RoundClientStatus.Open,
			ExternalRoundId: query.ExternalRoundId,
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			PlayerWinLoss: 0.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.UnixEpoch,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository
			.GetAsync(
			id: Arg.Is(roundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAdd>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Is(new Models.RoundOpen(
				Bet: query.Bet,
				ValidBet: query.ValidBet)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundClosed>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundReopen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Reopen()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalRoundId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalClosedOn = DateTimeOffset.UtcNow;
		var status = Common.Enums.RoundStatus.Closed;
		var clientStatus = Common.Enums.RoundClientStatus.Win;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			Status: status,
			ClientStatus: clientStatus,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository
			.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalRoundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfo(
			Id: roundId,
			GameProviderId: query.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: query.GameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: query.PlayerCode,
			WalletId: query.WalletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			ExternalRoundId: query.ExternalRoundId,
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
		_ = fakeRoundRepository
			.GetAsync(
			id: Arg.Is(roundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAdd>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundOpen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundClosed>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Is(new Models.RoundReopen(
				ClientStatus: Common.Enums.RoundClientStatus.Win,
				ExternalClosedOn: externalClosedOn,
				Bet: query.Bet,
				ValidBet: query.ValidBet,
				Settle: query.Settle,
				Commission: query.Commission,
				Turnover: query.Turnover,
				PlayerWinLoss: query.PlayerWinLoss)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Reopen_Is_RoundStatus_Open()
	{
		var fakeLogger = NullLogger<RoundCommandGetService>.Instance;
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalRoundId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow.AddSeconds(-5);
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var status = Common.Enums.RoundStatus.Open;
		var clientStatus = Common.Enums.RoundClientStatus.Open;

		var query = new RoundCommand(
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: gameId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: playerCode,
			CurrencyCode: "CNY",
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			Status: status,
			ClientStatus: clientStatus,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			PlayerWinLoss: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository
			.GetIdAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalRoundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfo(
			Id: roundId,
			GameProviderId: query.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: query.GameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: query.PlayerCode,
			WalletId: query.WalletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Win,
			ExternalRoundId: query.ExternalRoundId,
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: 10.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.Now,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository
			.GetAsync(
			id: Arg.Is(roundId),
			externalCreatedOn: Arg.Is(query.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandGetService(
			fakeLogger,
			fakeSerialNumberGenerator,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAdd>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundOpen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundClosed>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			id: Arg.Is(roundId),
			source: Arg.Any<Models.RoundReopen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(query.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
