using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.IntegrationTests
{
    public partial class GameRoundsClientUnitTest
    {
        [TestMethod]
        public async Task GameRounds_UpdateRoundClosed_ExternalRoundId_Not_Exist()
        {
            var roundId = Guid.NewGuid();

            var model = new UpdateRoundClosedViewModel(
                ExternalClosedOn: DateTimeOffset.UtcNow,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                SettleAmount: 0,
                Commission: 0,
                Turnover: 10m,
                PlayerWinLoss: -10m,
                ClientStatus: GameRoundClientStatus.Loss,
                AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            var web = CreateTestSettingSite(services =>
            {
                var fakeGameRound = Substitute.For<IGameRound>();
                _ = fakeGameRound.ExistAsync(roundId).Returns(ValueTask.FromResult(false));
                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddSingleton(fakeGameRoundRecord);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.UpdateRoundClosedAsync(roundId, model).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual.Success);
            Assert.AreEqual(HttpStatusCode.InternalServerError, actual.HttpStatusCode);

            _ = fakeGameRoundRecord.Received(0).AddAsync(Arg.Any<GameRoundInfo>());
        }

        [TestMethod]
        public async Task GameRounds_UpdateRoundClosed_Normal_Process()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var model = new UpdateRoundClosedViewModel(
                ExternalClosedOn: DateTimeOffset.UtcNow,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                SettleAmount: 0,
                Commission: 0,
                Turnover: 10m,
                PlayerWinLoss: -10m,
                ClientStatus: GameRoundClientStatus.Loss,
                AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            var web = CreateTestSettingSite(services =>
            {
                var fakeGameRound = Substitute.For<IGameRound>();

                _ = fakeGameRound.ExistAsync(roundId).Returns(ValueTask.FromResult(true));

                _ = fakeGameRound.UpdateRoundClosedAsync(new UpdateRoundClosed(
                    RoundId: roundId,
                    ExternalClosedOn: model.ExternalClosedOn,
                    BetAmount: model.BetAmount,
                    ValidBetAmount: model.ValidBetAmount,
                    SettleAmount: model.SettleAmount,
                    Commission: model.Commission,
                    Turnover: model.Turnover,
                    PlayerWinLoss: model.PlayerWinLoss,
                    ClientStatus: model.ClientStatus,
                    AdditionalData: model.AdditionalData));

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
                    ExternalClosedOn: model.ExternalClosedOn,
                    BetAmount: model.BetAmount,
                    ValidBetAmount: model.ValidBetAmount,
                    SettleAmount: model.SettleAmount,
                    Commission: model.Commission,
                    Turnover: model.Turnover,
                    CompanyWinLoss: -model.PlayerWinLoss,
                    TieBetAdjustment: 0,
                    ClientStatus: GameRoundClientStatus.Open,
                    ReopenTurnoverAdjustment: null,
                    ReopenedOn: null,
                    EffectiveBettingPercentage: null,
                    AdditionalData: model.AdditionalData ?? string.Empty);

                _ = fakeGameRound.GetAsync(roundId).Returns(ValueTask.FromResult<GameRoundInfo?>(gameRound));

                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddSingleton(fakeGameRoundRecord);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.UpdateRoundClosedAsync(roundId, model).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(HttpStatusCode.OK, actual.HttpStatusCode);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Any<GameRoundInfo>());
        }
    }
}
