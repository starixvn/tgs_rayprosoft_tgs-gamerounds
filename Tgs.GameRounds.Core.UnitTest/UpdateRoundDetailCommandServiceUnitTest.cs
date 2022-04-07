using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;

namespace Tgs.GameRounds.Core.UnitTest
{
    [TestClass]
    public class UpdateRoundDetailCommandServiceUnitTest
    {
        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameRoundCache = Substitute.For<IGameRoundService>();
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRoundRepository>();

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

            var data = new UpdateRoundDetailCommand(
                RoundId: roundId,
                AdditionalData: "{\"id\":10531704494,\"tableId\":10102,\"shoeId\":17,\"playId\":34,\"lobbyId\":1,\"gameType\":1,\"gameId\":1,\"memberId\":73304209,\"parentId\":19377,\"betTime\":\"2022-03-1709:19:38\",\"calTime\":\"2022-03-1709:20:01\",\"winOrLoss\":40.0,\"balanceBefore\":6020.41,\"betPoints\":20.0,\"betPointsz\":0.0,\"availableBet\":20.0,\"userName\":\"4UDW8YGX311V\",\"result\":\"{\\u0022result\\u0022:\\u00225,2,7\\u0022,\\u0022poker\\u0022:{\\u0022banker\\u0022:\\u002238-36-36\\u0022,\\u0022player\\u0022:\\u002246-36-0\\u0022}}\",\"betDetail\":\"{\\u0022player\\u0022:20.0,\\u0022playerW\\u0022:40.0}\",\"ip\":\"220.132.206.70\",\"ext\":\"220317B020851\",\"isRevocation\":1,\"currencyId\":1,\"deviceType\":5,\"serial\":\"-10531704494\",\"roadid\":0,\"pluginid\":0}"
                );
            var expected = Guid.NewGuid();

            var gameRound = new GameRoundInfo(
                RoundId: expected,
                ExternalRoundId: externalRoundId,
                GameProviderId: gameProviderId,
                PlayerCode: playerCode,
                GameId: gameId,
                CurrencyCode: "CNY",
                Status: GameRoundStatus.Open,
                GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId: walletId,
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: null,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn: DateTime.UtcNow,
                ExternalClosedOn: null,
                BetAmount: null,
                ValidBetAmount: null,
                SettleAmount: null,
                Commission: null,
                Turnover: null,
                WinLoss: null,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Open,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                EffectiveBettingPercentage: null,
                AdditionalData: "{\"id\":10531704494,\"tableId\":10102,\"shoeId\":17,\"playId\":34,\"lobbyId\":1,\"gameType\":1,\"gameId\":1,\"memberId\":73304209,\"parentId\":19377,\"betTime\":\"2022-03-1709:19:38\",\"calTime\":\"2022-03-1709:20:01\",\"winOrLoss\":40.0,\"balanceBefore\":6020.41,\"betPoints\":20.0,\"betPointsz\":0.0,\"availableBet\":20.0,\"userName\":\"4UDW8YGX311V\",\"result\":\"{\\u0022result\\u0022:\\u00225,2,7\\u0022,\\u0022poker\\u0022:{\\u0022banker\\u0022:\\u002238-36-36\\u0022,\\u0022player\\u0022:\\u002246-36-0\\u0022}}\",\"betDetail\":\"{\\u0022player\\u0022:20.0,\\u0022playerW\\u0022:40.0}\",\"ip\":\"220.132.206.70\",\"ext\":\"220317B020851\",\"isRevocation\":1,\"currencyId\":1,\"deviceType\":5,\"serial\":\"-10531704494\",\"roadid\":0,\"pluginid\":0}"
                );

            var service = new UpdateRoundDetailCommandService(
                fakeGameRoundCache,
                fakeGameRoundRecord,
                fakeGameRound);

            _ = fakeGameRoundCache.ExistAsync(data.RoundId).Returns(ValueTask.FromResult<bool>(true));

            _ = fakeGameRound.UpdateRoundDetailAsync(data.RoundId, data.AdditionalData);

            _ = fakeGameRound.GetAsync(data.RoundId).Returns(ValueTask.FromResult(gameRound));

            await service.ExecuteAsync(data).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Is(gameRound));
        }
    }
}
