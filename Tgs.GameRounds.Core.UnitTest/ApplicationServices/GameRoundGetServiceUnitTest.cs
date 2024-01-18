using System;
using System.Text.Json.Nodes;
using System.Threading;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.UnitTest.ApplicationServices;

public class GameRoundGetServiceUnitTest
{
    [Fact]
    public async void Is_Null()
    {
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameRoundRepository = Substitute.For<IGameRoundRepository>();

        var query = new GameRoundQuery(
            RoundId: Guid.NewGuid());

        _ = fakeGameRoundRepository.GetAsync(
            id: Arg.Is(query.RoundId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((GameRoundInfo?)null);

        var sut = new GameRoundGetService(
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

        var query = new GameRoundQuery(
            RoundId: Guid.NewGuid());

        var gameRoundInfo = new GameRoundInfo(
            RoundId: query.RoundId,
            ExternalRoundId: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            GameProviderId: Guid.NewGuid(),
            PlayerCode: "test",
            GameId: 1,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Closed,
            GameProviderConfigurationId: Guid.NewGuid(),
            WalletId: Guid.NewGuid(),
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
            id: Arg.Is(query.RoundId),
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
                ["platformTxId"] = gameRoundInfo.ExternalRoundId
            }.ToJsonString());
        _ = fakeGameRoundRepository.GetRoundDetailAsync(
            id: gameRoundInfo.RoundId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameRoundDetailInfo);

        var sut = new GameRoundGetService(
           fakeGameRepository,
           fakeGameProviderRepository,
           fakeGameRoundRepository);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(gameRoundInfo.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(externalGameId, actual.ExternalGameId);
    }
}
