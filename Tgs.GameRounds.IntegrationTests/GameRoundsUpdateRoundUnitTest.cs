using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.IntegrationTests
{
    public partial class GameRoundsClientUnitTest
    {
        [TestMethod]
        public async Task GameRounds_UpdateRound_ExternalRoundId_Not_Exist()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var roundId = Guid.NewGuid();

            var viewModel = new UpdateRoundViewModel()
            {
                Status = GameRoundStatus.Closed,
                ClosedOn = DateTimeOffset.UtcNow,
                ExternalClosedOn = DateTimeOffset.UtcNow,
                BetAmount = 20m,
                ValidBetAmount = 20m,
                SettleAmount = 40m,
                Commission = 0m,
                Turnover = 20m,
                WinLoss = -20m,
                TieBetAdjustment = null,
                ClientStatus = GameRoundClientStatus.Win,
                ReopenTurnoverAdjustment = null,
                ReopenedOn = null,
                AdditionalData = null
            };

            var expected = false;

            var web = CreateTestSettingSite(services =>
            {
                var fakeGameRound = Substitute.For<IGameRound>();
                _ = fakeGameRound.ExistAsync(roundId).Returns(ValueTask.FromResult(false));
                _ = services.AddSingleton(fakeGameRound);

                var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
                _ = services.AddSingleton(fakeGameRoundRecord);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.UpdateRoundAsync(roundId, viewModel).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task GameRounds_UpdateRound_Normal_Process()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var data = new UpdateRoundViewModel()
            {
                Status = GameRoundStatus.Closed,
                ClosedOn = DateTimeOffset.UtcNow,
                ExternalClosedOn = DateTimeOffset.UtcNow,
                BetAmount = 20m,
                ValidBetAmount = 20m,
                SettleAmount = 40m,
                Commission = 0m,
                Turnover = 20m,
                WinLoss = -20m,
                TieBetAdjustment = null,
                ClientStatus = GameRoundClientStatus.Win,
                ReopenTurnoverAdjustment = null,
                ReopenedOn = null,
                AdditionalData = null
            };

            var expected = true;

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            var web = CreateTestSettingSite(services =>
            {
                var fakeGameRound = Substitute.For<IGameRound>();

                _ = fakeGameRound.ExistAsync(roundId).Returns(ValueTask.FromResult(true));

                _ = fakeGameRound.UpdateRoundAsync(new(
                    roundId,
                    data.Status,
                    data.ClosedOn,
                    data.ExternalClosedOn,
                    data.BetAmount,
                    data.ValidBetAmount,
                    data.SettleAmount,
                    data.Commission,
                    data.Turnover,
                    data.WinLoss,
                    data.TieBetAdjustment,
                    data.ClientStatus,
                    data.ReopenTurnoverAdjustment,
                    data.ReopenedOn,
                    data.AdditionalData));

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
                    ExternalClosedOn: data.ExternalClosedOn,
                    BetAmount: data.BetAmount,
                    ValidBetAmount: data.ValidBetAmount,
                    SettleAmount: data.SettleAmount,
                    Commission: data.Commission,
                    Turnover: data.Turnover,
                    WinLoss: data.WinLoss,
                    TieBetAdjustment: data.TieBetAdjustment,
                    ClientStatus: GameRoundClientStatus.Open,
                    ReopenTurnoverAdjustment: data.ReopenTurnoverAdjustment,
                    ReopenedOn: data.ReopenedOn,
                    EffectiveBettingPercentage: null,
                    AdditionalData: data.AdditionalData ?? string.Empty);

                _ = fakeGameRound.GetAsync(roundId).Returns(ValueTask.FromResult<GameRoundInfo?>(gameRound));

                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddSingleton(fakeGameRoundRecord);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.UpdateRoundAsync(roundId, data).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Any<GameRoundInfo>());
        }
    }
}
