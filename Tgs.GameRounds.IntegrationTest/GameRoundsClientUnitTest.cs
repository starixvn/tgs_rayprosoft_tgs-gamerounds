using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;
using System.Text.Json.Nodes;
using Tgs.GameRounds.Common.ViewModels;
using MediatR;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.IntegrationTest;

public partial class GameRoundsClientUnitTest
{
	[Fact]
	public async Task Round_Get_ExternalCreatedOn_Is_Null()
	{
		var roundId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "DG";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalGameId = "baccarat";

		var externalCreatedOn = new DateTimeOffset(2024, 04, 24, 15, 30, 45, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 00, TimeSpan.FromHours(+8));
		var createdOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var closedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var updatedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));

		var roundInfo = new RoundInfoResponse(
			RoundId: roundId,
			GameProviderId: gameProviderId,
			GameProviderCode: gameProviderCode,
			GameId: gameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: walletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			GameProviderConfigurationId: Guid.NewGuid(),
			ExternalRoundId: externalRoundId,
			ExternalGameId: externalGameId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			BetAmount: 10.0m,
			ValidBetAmount: 10.0m,
			SettleAmount: 0.0m,
			Commission: 0.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			UpdatedOn: updatedOn,
			AdditionalData: JsonSerializer.Serialize(new { roundId }));

		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is(new RoundGetRequest(
					RoundId: roundId,
					ExternalCreatedOn: null)),
				cancellationToken: Arg.Any<CancellationToken>())
				.Returns(roundInfo);

			_ = services.AddSingleton(fakeMediator);
		}));

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.RoundGetAsync(roundId);

		Assert.NotNull(actual);
		Assert.Equal(roundId, actual.RoundId);
		Assert.Equal(gameProviderId, actual.GameProviderId);
		Assert.Equal(gameProviderCode, actual.GameProviderCode);
		Assert.Equal(roundInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(roundInfo.GameId, actual.GameId);
		Assert.Equal(roundInfo.BrandId, actual.BrandId);
		Assert.Equal(roundInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(walletId, actual.WalletId);
		Assert.Equal(roundInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(roundInfo.Status, actual.Status);
		Assert.Equal(roundInfo.ClientStatus, actual.ClientStatus);
		Assert.Equal(externalRoundId, actual.ExternalRoundId);
		Assert.Equal(externalGameId, actual.ExternalGameId);
		Assert.Equal(roundInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(roundInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(roundInfo.BetAmount, actual.BetAmount);
		Assert.Equal(roundInfo.ValidBetAmount, actual.ValidBetAmount);
		Assert.Equal(roundInfo.SettleAmount, actual.SettleAmount);
		Assert.Equal(roundInfo.Commission, actual.Commission);
		Assert.Equal(roundInfo.PlayerWinLoss, actual.PlayerWinLoss);
		Assert.Equal(-roundInfo.PlayerWinLoss, actual.CompanyWinLoss);
		Assert.Equal(roundInfo.CreatedOn, actual.CreatedOn);
		Assert.Equal(roundInfo.ClosedOn, actual.ClosedOn);
		Assert.Equal(roundInfo.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(roundInfo.AdditionalData, actual.AdditionalData);
	}

	[Fact]
	public async Task Round_Get_ExternalCreatedOn_Not_Null()
	{
		var roundId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "DG";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalGameId = "baccarat";

		var externalCreatedOn = new DateTimeOffset(2024, 04, 24, 15, 30, 45, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 00, TimeSpan.FromHours(+8));
		var createdOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var closedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var updatedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));

		var roundInfo = new RoundInfoResponse(
			RoundId: roundId,
			GameProviderId: gameProviderId,
			GameProviderCode: gameProviderCode,
			GameId: gameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: walletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			GameProviderConfigurationId: Guid.NewGuid(),
			ExternalRoundId: externalRoundId,
			ExternalGameId: externalGameId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			BetAmount: 10.0m,
			ValidBetAmount: 10.0m,
			SettleAmount: 0.0m,
			Commission: 0.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			UpdatedOn: updatedOn,
			AdditionalData: JsonSerializer.Serialize(new { roundId }));

		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is(new RoundGetRequest(
					RoundId: roundId,
					ExternalCreatedOn: externalCreatedOn)),
				cancellationToken: Arg.Any<CancellationToken>())
				.Returns(roundInfo);

			_ = services.AddSingleton(fakeMediator);
		}));

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.RoundGetAsync(roundId, externalCreatedOn);

		Assert.NotNull(actual);
		Assert.Equal(roundId, actual.RoundId);
		Assert.Equal(gameProviderId, actual.GameProviderId);
		Assert.Equal(gameProviderCode, actual.GameProviderCode);
		Assert.Equal(roundInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(roundInfo.GameId, actual.GameId);
		Assert.Equal(roundInfo.BrandId, actual.BrandId);
		Assert.Equal(roundInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(walletId, actual.WalletId);
		Assert.Equal(roundInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(roundInfo.Status, actual.Status);
		Assert.Equal(roundInfo.ClientStatus, actual.ClientStatus);
		Assert.Equal(externalRoundId, actual.ExternalRoundId);
		Assert.Equal(externalGameId, actual.ExternalGameId);
		Assert.Equal(roundInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(roundInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(roundInfo.BetAmount, actual.BetAmount);
		Assert.Equal(roundInfo.ValidBetAmount, actual.ValidBetAmount);
		Assert.Equal(roundInfo.SettleAmount, actual.SettleAmount);
		Assert.Equal(roundInfo.Commission, actual.Commission);
		Assert.Equal(roundInfo.PlayerWinLoss, actual.PlayerWinLoss);
		Assert.Equal(-roundInfo.PlayerWinLoss, actual.CompanyWinLoss);
		Assert.Equal(roundInfo.CreatedOn, actual.CreatedOn);
		Assert.Equal(roundInfo.ClosedOn, actual.ClosedOn);
		Assert.Equal(roundInfo.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(roundInfo.AdditionalData, actual.AdditionalData);
	}

	[Fact]
	public async Task Round_Get_Data_Is_Empty()
	{
		var roundId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");

		var externalCreatedOn = new DateTimeOffset(2024, 04, 24, 15, 30, 45, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 00, TimeSpan.FromHours(+8));
		var createdOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var closedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var updatedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));

		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is(new RoundGetRequest(
					RoundId: roundId,
					ExternalCreatedOn: externalCreatedOn)),
				cancellationToken: Arg.Any<CancellationToken>())
				.Returns((RoundInfoResponse?)null);

			_ = services.AddSingleton(fakeMediator);
		}));

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.RoundGetAsync(roundId, externalCreatedOn);

		Assert.Null(actual);
	}

	[Fact]
	public async Task Round_Get_ByExternalGame_ExternalCreatedOn_Is_Null()
	{
		var roundId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "DG";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalGameId = "baccarat";

		var externalCreatedOn = new DateTimeOffset(2024, 04, 24, 15, 30, 45, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 00, TimeSpan.FromHours(+8));
		var createdOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var closedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var updatedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));

		var roundInfo = new RoundInfoResponse(
			RoundId: roundId,
			GameProviderId: gameProviderId,
			GameProviderCode: gameProviderCode,
			GameId: gameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: walletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			GameProviderConfigurationId: Guid.NewGuid(),
			ExternalRoundId: externalRoundId,
			ExternalGameId: externalGameId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			BetAmount: 10.0m,
			ValidBetAmount: 10.0m,
			SettleAmount: 0.0m,
			Commission: 0.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			UpdatedOn: updatedOn,
			AdditionalData: JsonSerializer.Serialize(new { roundId }));

		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			var request = new RoundGetExternalGameRequest(
				GameProviderId: gameProviderId,
				WalletId: walletId,
				ExternalRoundId: externalRoundId,
				ExternalGameId: externalGameId,
				ExternalCreatedOn: null);
			_ = fakeMediator.Send(
				request: Arg.Is(request),
				cancellationToken: Arg.Any<CancellationToken>())
				.Returns(roundInfo);

			_ = services.AddSingleton(fakeMediator);
		}));

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.RoundGetAsync(gameProviderId, walletId, externalRoundId, externalGameId);

		Assert.NotNull(actual);
		Assert.Equal(roundId, actual.RoundId);
		Assert.Equal(gameProviderId, actual.GameProviderId);
		Assert.Equal(gameProviderCode, actual.GameProviderCode);
		Assert.Equal(roundInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(roundInfo.GameId, actual.GameId);
		Assert.Equal(roundInfo.BrandId, actual.BrandId);
		Assert.Equal(roundInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(walletId, actual.WalletId);
		Assert.Equal(roundInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(roundInfo.Status, actual.Status);
		Assert.Equal(roundInfo.ClientStatus, actual.ClientStatus);
		Assert.Equal(externalRoundId, actual.ExternalRoundId);
		Assert.Equal(externalGameId, actual.ExternalGameId);
		Assert.Equal(roundInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(roundInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(roundInfo.BetAmount, actual.BetAmount);
		Assert.Equal(roundInfo.ValidBetAmount, actual.ValidBetAmount);
		Assert.Equal(roundInfo.SettleAmount, actual.SettleAmount);
		Assert.Equal(roundInfo.Commission, actual.Commission);
		Assert.Equal(roundInfo.PlayerWinLoss, actual.PlayerWinLoss);
		Assert.Equal(-roundInfo.PlayerWinLoss, actual.CompanyWinLoss);
		Assert.Equal(roundInfo.CreatedOn, actual.CreatedOn);
		Assert.Equal(roundInfo.ClosedOn, actual.ClosedOn);
		Assert.Equal(roundInfo.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(roundInfo.AdditionalData, actual.AdditionalData);
	}

	[Fact]
	public async Task Round_Get_ByExternalGame_ExternalCreatedOn_Not_Null()
	{
		var roundId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "DG";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalGameId = "baccarat";

		var externalCreatedOn = new DateTimeOffset(2024, 04, 24, 15, 30, 45, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 00, TimeSpan.FromHours(+8));
		var createdOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var closedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var updatedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));

		var roundInfo = new RoundInfoResponse(
			RoundId: roundId,
			GameProviderId: gameProviderId,
			GameProviderCode: gameProviderCode,
			GameId: gameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: walletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			GameProviderConfigurationId: Guid.NewGuid(),
			ExternalRoundId: externalRoundId,
			ExternalGameId: externalGameId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			BetAmount: 10.0m,
			ValidBetAmount: 10.0m,
			SettleAmount: 0.0m,
			Commission: 0.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			UpdatedOn: updatedOn,
			AdditionalData: JsonSerializer.Serialize(new { roundId }));

		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			var request = new RoundGetExternalGameRequest(
				GameProviderId: gameProviderId,
				WalletId: walletId,
				ExternalRoundId: externalRoundId,
				ExternalGameId: externalGameId,
				ExternalCreatedOn: externalCreatedOn);
			_ = fakeMediator.Send(
				request: Arg.Is(request),
				cancellationToken: Arg.Any<CancellationToken>())
				.Returns(roundInfo);

			_ = services.AddSingleton(fakeMediator);
		}));

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.RoundGetAsync(gameProviderId, walletId, externalRoundId, externalGameId, externalCreatedOn);

		Assert.NotNull(actual);
		Assert.Equal(roundId, actual.RoundId);
		Assert.Equal(gameProviderId, actual.GameProviderId);
		Assert.Equal(gameProviderCode, actual.GameProviderCode);
		Assert.Equal(roundInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(roundInfo.GameId, actual.GameId);
		Assert.Equal(roundInfo.BrandId, actual.BrandId);
		Assert.Equal(roundInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(walletId, actual.WalletId);
		Assert.Equal(roundInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(roundInfo.Status, actual.Status);
		Assert.Equal(roundInfo.ClientStatus, actual.ClientStatus);
		Assert.Equal(externalRoundId, actual.ExternalRoundId);
		Assert.Equal(externalGameId, actual.ExternalGameId);
		Assert.Equal(roundInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(roundInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(roundInfo.BetAmount, actual.BetAmount);
		Assert.Equal(roundInfo.ValidBetAmount, actual.ValidBetAmount);
		Assert.Equal(roundInfo.SettleAmount, actual.SettleAmount);
		Assert.Equal(roundInfo.Commission, actual.Commission);
		Assert.Equal(roundInfo.PlayerWinLoss, actual.PlayerWinLoss);
		Assert.Equal(-roundInfo.PlayerWinLoss, actual.CompanyWinLoss);
		Assert.Equal(roundInfo.CreatedOn, actual.CreatedOn);
		Assert.Equal(roundInfo.ClosedOn, actual.ClosedOn);
		Assert.Equal(roundInfo.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(roundInfo.AdditionalData, actual.AdditionalData);
	}

	[Fact]
	public async Task Round_Get_ByExternalGame_Data_Is_Empty()
	{
		var gameProviderId = Guid.NewGuid();
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalGameId = "baccarat";

		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			var request = new RoundGetExternalGameRequest(
				GameProviderId: gameProviderId,
				WalletId: walletId,
				ExternalRoundId: externalRoundId,
				ExternalGameId: externalGameId,
				ExternalCreatedOn: null);
			_ = fakeMediator.Send(
				request: Arg.Is(request),
				cancellationToken: Arg.Any<CancellationToken>())
				.Returns((RoundInfoResponse?)null);

			_ = services.AddSingleton(fakeMediator);
		}));

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.RoundGetAsync(gameProviderId, walletId, externalRoundId, externalGameId);

		Assert.Null(actual);
	}

	[Fact]
	public async Task Round_Get_ByGame_ExternalCreatedOn_Null()
	{
		var roundId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "DG";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalGameId = "baccarat";

		var externalCreatedOn = new DateTimeOffset(2024, 04, 24, 15, 30, 45, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 00, TimeSpan.FromHours(+8));
		var createdOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var closedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var updatedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));

		var roundInfo = new RoundInfoResponse(
			RoundId: roundId,
			GameProviderId: gameProviderId,
			GameProviderCode: gameProviderCode,
			GameId: gameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: walletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			GameProviderConfigurationId: Guid.NewGuid(),
			ExternalRoundId: externalRoundId,
			ExternalGameId: externalGameId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			BetAmount: 10.0m,
			ValidBetAmount: 10.0m,
			SettleAmount: 0.0m,
			Commission: 0.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			UpdatedOn: updatedOn,
			AdditionalData: JsonSerializer.Serialize(new { roundId }));

		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			var request = new RoundGetGameRequest(
				GameProviderId: gameProviderId,
				GameId: gameId,
				WalletId: walletId,
				ExternalRoundId: externalRoundId,
				ExternalCreatedOn: null);
			_ = fakeMediator.Send(
				request: Arg.Is(request),
				cancellationToken: Arg.Any<CancellationToken>())
				.Returns(roundInfo);

			_ = services.AddSingleton(fakeMediator);
		}));

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.RoundGetAsync(gameProviderId, gameId, walletId, externalRoundId);

		Assert.NotNull(actual);
		Assert.Equal(roundId, actual.RoundId);
		Assert.Equal(gameProviderId, actual.GameProviderId);
		Assert.Equal(gameProviderCode, actual.GameProviderCode);
		Assert.Equal(roundInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(roundInfo.GameId, actual.GameId);
		Assert.Equal(roundInfo.BrandId, actual.BrandId);
		Assert.Equal(roundInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(walletId, actual.WalletId);
		Assert.Equal(roundInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(roundInfo.Status, actual.Status);
		Assert.Equal(roundInfo.ClientStatus, actual.ClientStatus);
		Assert.Equal(externalRoundId, actual.ExternalRoundId);
		Assert.Equal(externalGameId, actual.ExternalGameId);
		Assert.Equal(roundInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(roundInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(roundInfo.BetAmount, actual.BetAmount);
		Assert.Equal(roundInfo.ValidBetAmount, actual.ValidBetAmount);
		Assert.Equal(roundInfo.SettleAmount, actual.SettleAmount);
		Assert.Equal(roundInfo.Commission, actual.Commission);
		Assert.Equal(roundInfo.PlayerWinLoss, actual.PlayerWinLoss);
		Assert.Equal(-roundInfo.PlayerWinLoss, actual.CompanyWinLoss);
		Assert.Equal(roundInfo.CreatedOn, actual.CreatedOn);
		Assert.Equal(roundInfo.ClosedOn, actual.ClosedOn);
		Assert.Equal(roundInfo.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(roundInfo.AdditionalData, actual.AdditionalData);
	}

	[Fact]
	public async Task Round_Get_ByGame_ExternalCreatedOn_Not_Null()
	{
		var roundId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "DG";
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");
		var externalGameId = "baccarat";

		var externalCreatedOn = new DateTimeOffset(2024, 04, 24, 15, 30, 45, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 00, TimeSpan.FromHours(+8));
		var createdOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var closedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var updatedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));

		var roundInfo = new RoundInfoResponse(
			RoundId: roundId,
			GameProviderId: gameProviderId,
			GameProviderCode: gameProviderCode,
			GameId: gameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: walletId,
			CurrencyCode: "CNY",
			Status: Common.Enums.RoundStatus.Closed,
			ClientStatus: Common.Enums.RoundClientStatus.Loss,
			GameProviderConfigurationId: Guid.NewGuid(),
			ExternalRoundId: externalRoundId,
			ExternalGameId: externalGameId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			BetAmount: 10.0m,
			ValidBetAmount: 10.0m,
			SettleAmount: 0.0m,
			Commission: 0.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			UpdatedOn: updatedOn,
			AdditionalData: JsonSerializer.Serialize(new { roundId }));

		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			var request = new RoundGetGameRequest(
				GameProviderId: gameProviderId,
				GameId: gameId,
				WalletId: walletId,
				ExternalRoundId: externalRoundId,
				ExternalCreatedOn: externalCreatedOn);
			_ = fakeMediator.Send(
				request: Arg.Is(request),
				cancellationToken: Arg.Any<CancellationToken>())
				.Returns(roundInfo);

			_ = services.AddSingleton(fakeMediator);
		}));

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.RoundGetAsync(gameProviderId, gameId, walletId, externalRoundId, externalCreatedOn);

		Assert.NotNull(actual);
		Assert.Equal(roundId, actual.RoundId);
		Assert.Equal(gameProviderId, actual.GameProviderId);
		Assert.Equal(gameProviderCode, actual.GameProviderCode);
		Assert.Equal(roundInfo.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(roundInfo.GameId, actual.GameId);
		Assert.Equal(roundInfo.BrandId, actual.BrandId);
		Assert.Equal(roundInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(walletId, actual.WalletId);
		Assert.Equal(roundInfo.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(roundInfo.Status, actual.Status);
		Assert.Equal(roundInfo.ClientStatus, actual.ClientStatus);
		Assert.Equal(externalRoundId, actual.ExternalRoundId);
		Assert.Equal(externalGameId, actual.ExternalGameId);
		Assert.Equal(roundInfo.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(roundInfo.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(roundInfo.BetAmount, actual.BetAmount);
		Assert.Equal(roundInfo.ValidBetAmount, actual.ValidBetAmount);
		Assert.Equal(roundInfo.SettleAmount, actual.SettleAmount);
		Assert.Equal(roundInfo.Commission, actual.Commission);
		Assert.Equal(roundInfo.PlayerWinLoss, actual.PlayerWinLoss);
		Assert.Equal(-roundInfo.PlayerWinLoss, actual.CompanyWinLoss);
		Assert.Equal(roundInfo.CreatedOn, actual.CreatedOn);
		Assert.Equal(roundInfo.ClosedOn, actual.ClosedOn);
		Assert.Equal(roundInfo.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(roundInfo.AdditionalData, actual.AdditionalData);
	}

	[Fact]
	public async Task Round_Get_ByGame_Data_Is_Empty()
	{
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = Guid.NewGuid().ToString("N");

		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			var request = new RoundGetGameRequest(
				GameProviderId: gameProviderId,
				ExternalRoundId: externalRoundId,
				GameId: gameId,
				WalletId: walletId,
				ExternalCreatedOn: null);
			_ = fakeMediator.Send(
				request: Arg.Is(request),
				cancellationToken: Arg.Any<CancellationToken>())
				.Returns((RoundInfoResponse?)null);

			_ = services.AddSingleton(fakeMediator);
		}));

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.RoundGetAsync(gameProviderId, gameId, walletId, externalRoundId);

		Assert.Null(actual);
	}

	[Fact]
	public async Task RoundDetail_Get()
	{
		var roundId = Guid.NewGuid();
		var data = new RoundDetailInfo(
		   RoundId: roundId,
		   AdditionalData: JsonSerializer.Serialize(new
		   {
			   roundId
		   }));

		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRoundRepository = Substitute.For<IRoundRepository>();

			_ = fakeRoundRepository
			.GetRoundDetailAsync(
				id: Arg.Is(roundId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<RoundDetailInfo?>(data));

			_ = services.AddSingleton(fakeRoundRepository);
		}));

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.RoundDetailGetAsync(roundId);

		Assert.NotNull(actual);
		Assert.Equal(roundId, actual.RoundId);
	}

	[Fact]
	public async Task Round_Command_Id_Is_Null()
	{
		var externalRoundId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var source = new RoundCommandViewModel
		{
			GameProviderId = Guid.NewGuid(),
			GameProviderConfigurationId = Guid.NewGuid(),
			GameId = 1,
			BrandId = Guid.NewGuid(),
			PlayerCode = "test",
			WalletId = Guid.NewGuid(),
			CurrencyCode = "CNY",
			Status = Common.Enums.RoundStatus.Closed,
			ClientStatus = Common.Enums.RoundClientStatus.Win,
			ExternalRoundId = externalRoundId,
			ExternalCreatedOn = DateTimeOffset.UtcNow.AddSeconds(-5),
			ExternalClosedOn = DateTimeOffset.UtcNow,
			BetAmount = 10.0m,
			ValidBetAmount = 10.0m,
			SettleAmount = 20.0m,
			Commission = 0.0m,
			PlayerWinLoss = 10.0m,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			}
		};

		var roundId = Guid.NewGuid();
		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();
			var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
			var fakeTimeProvider = Substitute.For<TimeProvider>();

			_ = fakeSerialNumberGenerator.GenerateAsync(
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(roundId);

			var createdOn = new DateTimeOffset(2024, 09, 16, 11, 36, 57, TimeSpan.FromHours(+08));
			_ = fakeTimeProvider.GetUtcNow()
			.Returns(createdOn);

			_ = fakeMediator.Send(
				request: Arg.Is<RoundCommandRequest>(compare => compare.Id == roundId &&
					compare.CreatedOn == createdOn &&
					compare.GameProviderId == source.GameProviderId &&
					compare.GameProviderConfigurationId == source.GameProviderConfigurationId &&
					compare.GameId == source.GameId &&
					compare.BrandId == source.BrandId &&
					compare.PlayerCode == source.PlayerCode &&
					compare.WalletId == source.WalletId &&
					compare.CurrencyCode == source.CurrencyCode &&
					compare.Status == source.Status &&
					compare.ClientStatus == source.ClientStatus &&
					compare.ExternalRoundId == source.ExternalRoundId &&
					compare.ExternalCreatedOn == source.ExternalCreatedOn &&
					compare.ExternalClosedOn == source.ExternalClosedOn &&
					compare.Bet == source.BetAmount &&
					compare.ValidBet == source.ValidBetAmount &&
					compare.Settle == source.SettleAmount &&
					compare.Commission == source.Commission &&
					compare.PlayerWinLoss == source.PlayerWinLoss),
				cancellationToken:
				Arg.Any<CancellationToken>())
			.Returns(roundId);

			_ = services.AddTransient(_ => fakeMediator);
			_ = services.AddTransient(_ => fakeSerialNumberGenerator);
			_ = services.AddTransient(_ => fakeTimeProvider);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameRoundsClient(httpClient);

		var actual = await client.RoundCommandAsync(source);

		Assert.Equal(roundId, actual);
	}

	[Fact]
	public async Task Round_Command_Id_Is_Not_Null()
	{
		var externalRoundId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var source = new RoundCommandViewModel
		{
			Id = Guid.NewGuid(),
			GameProviderId = Guid.NewGuid(),
			GameProviderConfigurationId = Guid.NewGuid(),
			GameId = 1,
			BrandId = Guid.NewGuid(),
			PlayerCode = "test",
			WalletId = Guid.NewGuid(),
			CurrencyCode = "CNY",
			Status = Common.Enums.RoundStatus.Closed,
			ClientStatus = Common.Enums.RoundClientStatus.Win,
			ExternalRoundId = externalRoundId,
			ExternalCreatedOn = DateTimeOffset.UtcNow.AddSeconds(-5),
			ExternalClosedOn = DateTimeOffset.UtcNow,
			BetAmount = 10.0m,
			ValidBetAmount = 10.0m,
			SettleAmount = 20.0m,
			Commission = 0.0m,
			PlayerWinLoss = 10.0m,
			AdditionalData = new JsonObject
			{
				["ExternalRoundId"] = externalRoundId
			},
			CreatedOn = new DateTimeOffset(2024, 09, 16, 11, 36, 57, TimeSpan.FromHours(+08))
		};

		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();
			var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
			var fakeTimeProvider = Substitute.For<TimeProvider>();

			_ = fakeMediator.Send(
				request: Arg.Is<RoundCommandRequest>(compare => compare.Id == source.Id &&
					compare.CreatedOn == source.CreatedOn &&
					compare.GameProviderId == source.GameProviderId &&
					compare.GameProviderConfigurationId == source.GameProviderConfigurationId &&
					compare.GameId == source.GameId &&
					compare.BrandId == source.BrandId &&
					compare.PlayerCode == source.PlayerCode &&
					compare.WalletId == source.WalletId &&
					compare.CurrencyCode == source.CurrencyCode &&
					compare.Status == source.Status &&
					compare.ClientStatus == source.ClientStatus &&
					compare.ExternalRoundId == source.ExternalRoundId &&
					compare.ExternalCreatedOn == source.ExternalCreatedOn &&
					compare.ExternalClosedOn == source.ExternalClosedOn &&
					compare.Bet == source.BetAmount &&
					compare.ValidBet == source.ValidBetAmount &&
					compare.Settle == source.SettleAmount &&
					compare.Commission == source.Commission &&
					compare.PlayerWinLoss == source.PlayerWinLoss),
				cancellationToken:
				Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<Guid>(source.Id.Value));

			_ = services.AddTransient(_ => fakeMediator);
			_ = services.AddTransient(_ => fakeSerialNumberGenerator);
			_ = services.AddTransient(_ => fakeTimeProvider);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameRoundsClient(httpClient);

		var actual = await client.RoundCommandAsync(source);

		Assert.Equal(source.Id, actual);
	}
}
