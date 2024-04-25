using System;
using System.Text.Json.Nodes;
using System.Threading;
using NSubstitute;
using Tgs.GameRounds.Common.Enums;

using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.UnitTest.ApplicationServices;

public class RoundByGameGetServiceUnitTest
{
	[Fact]
	public async void ExternalCreatedOn_Is_Not_Null()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var roundId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "KY";
		var walletId = Guid.NewGuid();
		var externalRoundId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var gameId = 1;
		var externalGameId = "baccarat";
		var externalCreatedOn = new DateTimeOffset(2024, 04, 24, 15, 30, 45, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 00, TimeSpan.FromHours(+8));
		var createdOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var closedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var updatedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));

		var query = new RoundByGameQuery(
			GameProviderId: gameProviderId,
			GameId: gameId,
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalCreatedOn: externalCreatedOn);

		var roundInfo = new RoundInfo(
			Id: roundId,
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: gameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: walletId,
			CurrencyCode: "CNY",
			Status: RoundStatus.Closed,
			ClientStatus: RoundClientStatus.Loss,
			ExternalRoundId: externalRoundId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			UpdatedOn: updatedOn);
		_ = fakeRoundRepository.GetAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalRoundId),
			externalCreatedOn: Arg.Is(externalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(roundInfo);

		_ = fakeGameProviderRepository.GetCodeAsync(
			id: Arg.Is(roundInfo.GameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderCode);

		_ = fakeGameRepository.GetExternalIdAsync(
			id: Arg.Is(roundInfo.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(externalGameId);

		var gameRoundDetailInfo = new RoundDetailInfo(
			RoundId: roundInfo.Id,
			AdditionalData: new JsonObject
			{
				["platformTxId"] = query.ExternalRoundId
			}.ToJsonString());
		_ = fakeRoundRepository.GetRoundDetailAsync(
			id: roundInfo.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRoundDetailInfo);

		var sut = new RoundByGameGetService(
			fakeTimeProvider,
			fakeGameRepository,
			fakeGameProviderRepository,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

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
		Assert.Equal(roundInfo.Bet, actual.BetAmount);
		Assert.Equal(roundInfo.ValidBet, actual.ValidBetAmount);
		Assert.Equal(roundInfo.Settle, actual.SettleAmount);
		Assert.Equal(roundInfo.Commission, actual.Commission);
		Assert.Equal(roundInfo.Turnover, actual.Turnover);
		Assert.Equal(roundInfo.PlayerWinLoss, actual.PlayerWinLoss);
		Assert.Equal(roundInfo.CreatedOn, actual.CreatedOn);
		Assert.Equal(roundInfo.ClosedOn, actual.ClosedOn);
		Assert.Equal(roundInfo.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(gameRoundDetailInfo.AdditionalData, actual.AdditionalData);
	}

	[Fact]
	public async void ExternalCreatedOn_Is_Null()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var roundId = Guid.NewGuid();
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "KY";
		var walletId = Guid.NewGuid();
		var externalRoundId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var gameId = 1;
		var externalGameId = "baccarat";
		var externalCreatedOn = new DateTimeOffset(2024, 04, 24, 15, 30, 45, TimeSpan.FromHours(+8));
		var externalClosedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 00, TimeSpan.FromHours(+8));
		var createdOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var closedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));
		var updatedOn = new DateTimeOffset(2024, 04, 24, 15, 31, 15, TimeSpan.FromHours(+8));

		var query = new RoundByGameQuery(
			GameProviderId: gameProviderId,
			GameId: gameId,
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalCreatedOn: null);

		var currentTime = new DateTimeOffset(2024, 04, 24, 15, 30, 45, TimeSpan.FromHours(+8));
		_ = fakeTimeProvider.GetUtcNow()
			.Returns(currentTime);

		var roundInfo = new RoundInfo(
			Id: roundId,
			GameProviderId: gameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: gameId,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			WalletId: walletId,
			CurrencyCode: "CNY",
			Status: RoundStatus.Closed,
			ClientStatus: RoundClientStatus.Loss,
			ExternalRoundId: externalRoundId,
			ExternalCreatedOn: externalCreatedOn,
			ExternalClosedOn: externalClosedOn,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 10.0m,
			PlayerWinLoss: -10.0m,
			CreatedOn: createdOn,
			ClosedOn: closedOn,
			UpdatedOn: updatedOn);
		_ = fakeRoundRepository.GetAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalRoundId),
			externalCreatedOn: Arg.Is(currentTime.AddMonths(-3)),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(roundInfo);

		_ = fakeGameProviderRepository.GetCodeAsync(
			id: Arg.Is(roundInfo.GameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderCode);

		_ = fakeGameRepository.GetExternalIdAsync(
			id: Arg.Is(roundInfo.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(externalGameId);

		var gameRoundDetailInfo = new RoundDetailInfo(
			RoundId: roundInfo.Id,
			AdditionalData: new JsonObject
			{
				["platformTxId"] = query.ExternalRoundId
			}.ToJsonString());
		_ = fakeRoundRepository.GetRoundDetailAsync(
			id: roundInfo.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRoundDetailInfo);

		var sut = new RoundByGameGetService(
			fakeTimeProvider,
			fakeGameRepository,
			fakeGameProviderRepository,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

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
		Assert.Equal(roundInfo.Bet, actual.BetAmount);
		Assert.Equal(roundInfo.ValidBet, actual.ValidBetAmount);
		Assert.Equal(roundInfo.Settle, actual.SettleAmount);
		Assert.Equal(roundInfo.Commission, actual.Commission);
		Assert.Equal(roundInfo.Turnover, actual.Turnover);
		Assert.Equal(roundInfo.PlayerWinLoss, actual.PlayerWinLoss);
		Assert.Equal(roundInfo.CreatedOn, actual.CreatedOn);
		Assert.Equal(roundInfo.ClosedOn, actual.ClosedOn);
		Assert.Equal(roundInfo.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(gameRoundDetailInfo.AdditionalData, actual.AdditionalData);
	}

	[Fact]
	public async void Data_Is_Empty()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeRoundRepository = Substitute.For<IRoundRepository>();

		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalRoundId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var externalCreatedOn = new DateTimeOffset(2024, 04, 24, 15, 30, 45, TimeSpan.FromHours(+8));

		var query = new RoundByGameQuery(
			GameProviderId: gameProviderId,
			GameId: gameId,
			WalletId: walletId,
			ExternalRoundId: externalRoundId,
			ExternalCreatedOn: externalCreatedOn);

		_ = fakeRoundRepository.GetAsync(
			gameProviderId: Arg.Is(query.GameProviderId),
			gameId: Arg.Is(query.GameId),
			walletId: Arg.Is(query.WalletId),
			externalRoundId: Arg.Is(query.ExternalRoundId),
			externalCreatedOn: Arg.Is(externalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((RoundInfo?)null);

		var sut = new RoundByGameGetService(
			fakeTimeProvider,
			fakeGameRepository,
			fakeGameProviderRepository,
			fakeRoundRepository);

		var actual = await sut.GetAsync(query);

		Assert.Null(actual);
	}
}
