using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameRounds.Core.ApplicationServices;

namespace Tgs.GameRounds.Core.UnitTest.ApplicationServices;

public class RoundCommandServiceUnitTest
{
	[Fact]
	public async Task Add_Round_Status_Is_Open()
	{
		var fakeLogger = NullLogger<RoundCommandService>.Instance;
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var externalRoundId = Guid.NewGuid().ToString("N");
		var gameProviderId = Guid.NewGuid();
		var playerCode = "test";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalCreatedOn = DateTimeOffset.UtcNow;
		var status = Common.Enums.RoundStatus.Open;
		var clientStatus = Common.Enums.RoundClientStatus.Open;

		var request = new RoundCommandRequest(
			Id: Guid.NewGuid(),
			CreatedOn: DateTimeOffset.UtcNow,
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
			Bet: 20.0m,
			ValidBet: 20.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			PlayerWinLoss: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		_ = fakeRoundRepository.GetIdAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalRoundId: Arg.Is(request.ExternalRoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(null));

		var sut = new RoundCommandService(
			fakeLogger,
			fakeRoundRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;
		var actual = await sut.Handle(request, token);

		Assert.Equal(request.Id, actual);

		await fakeRoundRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is(new Models.RoundAdd(
				Id: request.Id,
				GameProviderId: request.GameProviderId,
				GameProviderConfigurationId: request.GameProviderConfigurationId,
				GameId: request.GameId,
				BrandId: request.BrandId,
				PlayerCode: request.PlayerCode,
				WalletId: request.WalletId,
				CurrencyCode: request.CurrencyCode,
				Status: Common.Enums.RoundStatus.Open,
				ClientStatus: Common.Enums.RoundClientStatus.Open,
				ExternalRoundId: request.ExternalRoundId,
				ExternalCreatedOn: externalCreatedOn,
				ExternalClosedOn: null,
				Bet: 20.0m,
				ValidBet: 20.0m,
				Settle: 0.0m,
				Commission: 0.0m,
				PlayerWinLoss: 0.0m,
				CreatedOn: request.CreatedOn)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundOpen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundClosed>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundReopen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(request.Id),
			additionalData: Arg.Is(request.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Add_Round_Status_Is_Closed()
	{
		var fakeLogger = NullLogger<RoundCommandService>.Instance;
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

		var request = new RoundCommandRequest(
			Id: Guid.NewGuid(),
			CreatedOn: DateTimeOffset.UtcNow,
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
			ValidBet: 0.5m,
			Settle: 19.5m,
			Commission: 0.0m,
			PlayerWinLoss: -0.5m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		_ = fakeRoundRepository.GetIdAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalRoundId: Arg.Is(request.ExternalRoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(null));

		var sut = new RoundCommandService(
			fakeLogger,
			fakeRoundRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;
		var actual = await sut.Handle(request, token);

		Assert.Equal(request.Id, actual);

		await fakeRoundRepository
			.Received(1)
			.AddAsync(
			source: Arg.Is(new Models.RoundAdd(
				Id: request.Id,
				GameProviderId: request.GameProviderId,
				GameProviderConfigurationId: request.GameProviderConfigurationId,
				GameId: request.GameId,
				BrandId: request.BrandId,
				PlayerCode: request.PlayerCode,
				WalletId: request.WalletId,
				CurrencyCode: request.CurrencyCode,
				Status: Common.Enums.RoundStatus.Closed,
				ClientStatus: Common.Enums.RoundClientStatus.Win,
				ExternalRoundId: request.ExternalRoundId,
				ExternalCreatedOn: externalCreatedOn,
				ExternalClosedOn: externalClosedOn,
				Bet: 20.0m,
				ValidBet: 0.5m,
				Settle: 19.5m,
				Commission: 0.0m,
				PlayerWinLoss: -0.5m,
				CreatedOn: request.CreatedOn)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundOpen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundClosed>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundReopen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(request.Id),
			additionalData: Arg.Is(request.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Closed()
	{
		var fakeLogger = NullLogger<RoundCommandService>.Instance;
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

		var request = new RoundCommandRequest(
			Id: Guid.NewGuid(),
			CreatedOn: DateTimeOffset.UtcNow,
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
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository.GetIdAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalRoundId: Arg.Is(request.ExternalRoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfo(
			Id: roundId,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: request.GameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: request.PlayerCode,
			WalletId: request.WalletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Open,
			ClientStatus: Common.Enums.RoundClientStatus.Open,
			ExternalRoundId: request.ExternalRoundId,
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			PlayerWinLoss: 0.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.UnixEpoch,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			id: Arg.Is(roundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandService(
			fakeLogger,
			fakeRoundRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;
		var actual = await sut.Handle(request, token);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAdd>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundOpen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.ClosedAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			id: Arg.Is(roundId),
			source: Arg.Is(new Models.RoundClosed(
				ClientStatus: Common.Enums.RoundClientStatus.Win,
				ExternalClosedOn: externalClosedOn,
				Bet: request.Bet,
				ValidBet: request.ValidBet,
				Settle: request.Settle,
				Commission: request.Commission,
				PlayerWinLoss: request.PlayerWinLoss)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundReopen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(request.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Closed_Get_DataBase_RoundStatus_Is_Closed()
	{
		var fakeLogger = NullLogger<RoundCommandService>.Instance;
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

		var request = new RoundCommandRequest(
			Id: Guid.NewGuid(),
			CreatedOn: DateTimeOffset.UtcNow,
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
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository.GetIdAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalRoundId: Arg.Is(request.ExternalRoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfo(
			Id: roundId,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: request.GameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: request.PlayerCode,
			WalletId: request.WalletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Win,
			ExternalRoundId: request.ExternalRoundId,
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			PlayerWinLoss: 10.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.Now,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			id: Arg.Is(roundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandService(
			fakeLogger,
			fakeRoundRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;
		var actual = await sut.Handle(request, token);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAdd>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundOpen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundClosed>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundReopen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.MergeDetailAsync(
			id: Arg.Any<Guid>(),
			additionalData: Arg.Is(request.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Open()
	{
		var fakeLogger = NullLogger<RoundCommandService>.Instance;
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

		var request = new RoundCommandRequest(
			Id: Guid.NewGuid(),
			CreatedOn: DateTimeOffset.UtcNow,
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
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository.GetIdAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalRoundId: Arg.Is(request.ExternalRoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfo(
			Id: roundId,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: request.GameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: request.PlayerCode,
			WalletId: request.WalletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Open,
			ClientStatus: Common.Enums.RoundClientStatus.Open,
			ExternalRoundId: request.ExternalRoundId,
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			PlayerWinLoss: 0.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.UnixEpoch,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			id: Arg.Is(roundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandService(
			fakeLogger,
			fakeRoundRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;
		var actual = await sut.Handle(request, token);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAdd>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.OpenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Is(roundId),
			source: Arg.Is(new Models.RoundOpen(
				Bet: request.Bet,
				ValidBet: request.ValidBet)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundClosed>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundReopen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(request.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Reopen()
	{
		var fakeLogger = NullLogger<RoundCommandService>.Instance;
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

		var request = new RoundCommandRequest(
			Id: Guid.NewGuid(),
			CreatedOn: DateTimeOffset.UtcNow,
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
			PlayerWinLoss: 10.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository.GetIdAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalRoundId: Arg.Is(request.ExternalRoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfo(
			Id: roundId,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: request.GameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: request.PlayerCode,
			WalletId: request.WalletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			ExternalRoundId: request.ExternalRoundId,
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.Now,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			id: Arg.Is(roundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandService(
			fakeLogger,
			fakeRoundRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;
		var actual = await sut.Handle(request, token);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAdd>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundOpen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundClosed>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.ReopenAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			id: Arg.Is(roundId),
			source: Arg.Is(new Models.RoundReopen(
				ClientStatus: Common.Enums.RoundClientStatus.Win,
				ExternalClosedOn: externalClosedOn,
				Bet: request.Bet,
				ValidBet: request.ValidBet,
				Settle: request.Settle,
				Commission: request.Commission,
				PlayerWinLoss: request.PlayerWinLoss)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(1)
			.MergeDetailAsync(
			id: Arg.Is(roundId),
			additionalData: Arg.Is(request.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Update_Round_Reopen_Is_RoundStatus_Open()
	{
		var fakeLogger = NullLogger<RoundCommandService>.Instance;
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

		var request = new RoundCommandRequest(
			Id: Guid.NewGuid(),
			CreatedOn: DateTimeOffset.UtcNow,
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
			PlayerWinLoss: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			});

		var roundId = Guid.NewGuid();
		_ = fakeRoundRepository.GetIdAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			gameProviderId: Arg.Is(request.GameProviderId),
			gameId: Arg.Is(request.GameId),
			walletId: Arg.Is(request.WalletId),
			externalRoundId: Arg.Is(request.ExternalRoundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<Guid?>(roundId));

		var gameRound = new Models.RoundInfo(
			Id: roundId,
			GameProviderId: request.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: request.GameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: request.PlayerCode,
			WalletId: request.WalletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Win,
			ExternalRoundId: request.ExternalRoundId,
			ExternalCreatedOn: DateTimeOffset.Now,
			ExternalClosedOn: DateTimeOffset.Now,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			PlayerWinLoss: 10.0m,
			CreatedOn: DateTimeOffset.Now,
			ClosedOn: DateTimeOffset.Now,
			UpdatedOn: DateTimeOffset.Now);
		_ = fakeRoundRepository.GetAsync(
			externalCreatedOn: Arg.Is(request.ExternalCreatedOn),
			id: Arg.Is(roundId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRound);

		var sut = new RoundCommandService(
			fakeLogger,
			fakeRoundRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;
		var actual = await sut.Handle(request, token);

		Assert.Equal(roundId, actual);

		await fakeRoundRepository
			.Received(0)
			.AddAsync(
			source: Arg.Any<Models.RoundAdd>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.OpenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundOpen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ClosedAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundClosed>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.ReopenAsync(
			externalCreatedOn: Arg.Any<DateTimeOffset>(),
			id: Arg.Any<Guid>(),
			source: Arg.Any<Models.RoundReopen>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRoundRepository
			.Received(0)
			.MergeDetailAsync(
			id: Arg.Any<Guid>(),
			additionalData: Arg.Is(request.AdditionalData.ToJsonString()!),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
