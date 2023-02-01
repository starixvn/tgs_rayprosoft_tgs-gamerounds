using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.IntegrationTests;

public partial class GameRoundsClientUnitTest
{
    [TestMethod]
    public async Task GameRounds_UpdateRoundDetail_ExternalRoundId_Not_Exist()
    {
        var roundId = Guid.NewGuid();

        var additionalData = "{\"id\":10531704494,\"tableId\":10102,\"shoeId\":17,\"playId\":34,\"lobbyId\":1,\"gameType\":1,\"gameId\":1,\"memberId\":73304209,\"parentId\":19377,\"betTime\":\"2022-03-1709:19:38\",\"calTime\":\"2022-03-1709:20:01\",\"winOrLoss\":40.0,\"balanceBefore\":6020.41,\"betPoints\":20.0,\"betPointsz\":0.0,\"availableBet\":20.0,\"userName\":\"4UDW8YGX311V\",\"result\":\"{\\u0022result\\u0022:\\u00225,2,7\\u0022,\\u0022poker\\u0022:{\\u0022banker\\u0022:\\u002238-36-36\\u0022,\\u0022player\\u0022:\\u002246-36-0\\u0022}}\",\"betDetail\":\"{\\u0022player\\u0022:20.0,\\u0022playerW\\u0022:40.0}\",\"ip\":\"220.132.206.70\",\"ext\":\"220317B020851\",\"isRevocation\":1,\"currencyId\":1,\"deviceType\":5,\"serial\":\"-10531704494\",\"roadid\":0,\"pluginid\":0}";

        var fakeGameRoundRecord = Substitute.For<IGameRoundRecord>();

        var web = CreateTestSettingSite(services =>
        {
            var fakeGameRound = Substitute.For<IGameRoundRepository>();
            _ = fakeGameRound.ExistAsync(roundId).Returns(ValueTask.FromResult(false));
            _ = services.AddSingleton(fakeGameRound);

            _ = services.AddSingleton(fakeGameRoundRecord);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.UpdateRoundDetailAsync(roundId, additionalData).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(false, actual.Success);
        Assert.AreEqual(HttpStatusCode.InternalServerError, actual.HttpStatusCode);
    }

    [TestMethod]
    public async Task GameRounds_UpdateRoundDetail_Normal_Process()
    {
        var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
        var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
        var gameId = 500001;
        var playerCode = "4ues8xikza7o";
        var roundId = Guid.NewGuid();

        var additionalData = "{\"id\":10531704494,\"tableId\":10102,\"shoeId\":17,\"playId\":34,\"lobbyId\":1,\"gameType\":1,\"gameId\":1,\"memberId\":73304209,\"parentId\":19377,\"betTime\":\"2022-03-1709:19:38\",\"calTime\":\"2022-03-1709:20:01\",\"winOrLoss\":40.0,\"balanceBefore\":6020.41,\"betPoints\":20.0,\"betPointsz\":0.0,\"availableBet\":20.0,\"userName\":\"4UDW8YGX311V\",\"result\":\"{\\u0022result\\u0022:\\u00225,2,7\\u0022,\\u0022poker\\u0022:{\\u0022banker\\u0022:\\u002238-36-36\\u0022,\\u0022player\\u0022:\\u002246-36-0\\u0022}}\",\"betDetail\":\"{\\u0022player\\u0022:20.0,\\u0022playerW\\u0022:40.0}\",\"ip\":\"220.132.206.70\",\"ext\":\"220317B020851\",\"isRevocation\":1,\"currencyId\":1,\"deviceType\":5,\"serial\":\"-10531704494\",\"roadid\":0,\"pluginid\":0}";

        var fakeGameRoundRecord = Substitute.For<IGameRoundRecord>();

        var web = CreateTestSettingSite(services =>
        {
            var fakeGameRound = Substitute.For<IGameRoundRepository>();

            _ = fakeGameRound.ExistAsync(roundId).Returns(ValueTask.FromResult(true));

            _ = fakeGameRound.UpdateRoundDetailAsync(
                roundId,
                additionalData);

            var gameRound = new GameRoundInfo(
                RoundId: roundId,
                ExternalRoundId: externalRoundId,
                GameProviderId: gameProviderId,
                PlayerCode: playerCode,
                GameId: gameId,
                CurrencyCode: "CNY",
                Status: GameRoundStatus.Open,
                GameProviderConfigurationId: Guid.NewGuid(),
                WalletId: Guid.NewGuid(),
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: null,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: Guid.NewGuid(),
                ExternalCreatedOn: DateTimeOffset.UtcNow,
                ExternalClosedOn: null,
                BetAmount: null,
                ValidBetAmount: null,
                SettleAmount: null,
                Commission: null,
                Turnover: null,
                CompanyWinLoss: null,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Open,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                EffectiveBettingPercentage: null);

            _ = fakeGameRound.GetAsync(roundId).Returns(ValueTask.FromResult<GameRoundInfo?>(gameRound));

            _ = services.AddSingleton(fakeGameRound);

            _ = services.AddSingleton(fakeGameRoundRecord);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.UpdateRoundDetailAsync(roundId, additionalData).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(true, actual.Success);
        Assert.AreEqual(HttpStatusCode.OK, actual.HttpStatusCode);
    }
}
