using System;
using System.Text.Json.Nodes;
using System.Threading;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.UnitTest.ApplicationServices;

public class GetGameRoundByGameGetServiceUnitTest
{
    [Fact]
    public async void Is_Null()
    {
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameRoundRepository = Substitute.For<IGameRoundRepository>();

        var query = new GetGameRoundByGameQuery(
            GameProviderId: Guid.NewGuid(),
            ExternalRoundId: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            GameId: 1,
            WalletId: Guid.NewGuid());

        _ = fakeGameRoundRepository.GetAsync(
            gameProviderId: Arg.Is(query.GameProviderId),
            externalRoundId: Arg.Is(query.ExternalRoundId),
            gameId: Arg.Is(query.GameId),
            walletId: Arg.Is(query.WalletId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((GameRoundInfo?)null);

        var sut = new GetGameRoundByGameGetService(
           fakeGameRepository,
           fakeGameProviderRepository,
           fakeGameRoundRepository);

        var actual = await sut.GetAsync(query);

        Assert.Null(actual);
    }

    [Fact]
    public async void Normal_Process()
    {
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameRoundRepository = Substitute.For<IGameRoundRepository>();

        var query = new GetGameRoundByGameQuery(
            GameProviderId: Guid.NewGuid(),
            ExternalRoundId: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            GameId: 1,
            WalletId: Guid.NewGuid());

        var gameRoundInfo = new GameRoundInfo(
            RoundId: Guid.NewGuid(),
            ExternalRoundId: query.ExternalRoundId,
            GameProviderId: query.GameProviderId,
            PlayerCode: "test",
            GameId: query.GameId,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Closed,
            GameProviderConfigurationId: Guid.NewGuid(),
            WalletId: query.WalletId,
            CreatedOn: DateTimeOffset.UtcNow,
            ClosedOn: DateTimeOffset.UtcNow,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: Guid.NewGuid(),
            ExternalCreatedOn: DateTimeOffset.UtcNow,
            ExternalClosedOn: DateTimeOffset.UtcNow,
            BetAmount: 10.0m,
            ValidBetAmount: 10.0m,
            SettleAmount: 20.0m,
            Commission: 0.0m,
            Turnover: 10.0m,
            CompanyWinLoss: -10.0m,
            TieBetAdjustment: 0,
            ClientStatus: GameRoundClientStatus.Win,
            ReopenTurnoverAdjustment: null,
            ReopenedOn: null,
            EffectiveBettingPercentage: null);
        _ = fakeGameRoundRepository.GetAsync(
            gameProviderId: Arg.Is(query.GameProviderId),
            externalRoundId: Arg.Is(query.ExternalRoundId),
            gameId: Arg.Is(query.GameId),
            walletId: Arg.Is(query.WalletId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameRoundInfo);

        var gameProviderCode = "KY";
        _ = fakeGameProviderRepository.GetCode(
            id: Arg.Is(gameRoundInfo.GameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderCode);

        var externalGameId = "baccarat";
        _ = fakeGameRepository.GetExternalIdAsync(
            id: Arg.Is(gameRoundInfo.GameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(externalGameId);

        var gameRoundDetailInfo = new GameRoundDetailInfo(
            RoundId: gameRoundInfo.RoundId,
            AdditionalData: new JsonObject
            {
                ["platformTxId"] = query.ExternalRoundId
            }.ToJsonString());
        _ = fakeGameRoundRepository.GetRoundDetailAsync(
            id: gameRoundInfo.RoundId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameRoundDetailInfo);

        var sut = new GetGameRoundByGameGetService(
           fakeGameRepository,
           fakeGameProviderRepository,
           fakeGameRoundRepository);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(query.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(externalGameId, actual.ExternalGameId);
    }
}
