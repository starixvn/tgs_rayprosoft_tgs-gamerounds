using System;
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
        public async Task GameRounds_UpdateRoundReopen_Normal_Process()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var model = new UpdateRoundReopenViewModel(
                Status: GameRoundStatus.Closed,
                ExternalClosedOn: DateTimeOffset.UtcNow,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                SettleAmount: 20m,
                Commission: 0,
                Turnover: 10m,
                WinLoss: 10m,
                ClientStatus: GameRoundClientStatus.Win,
                AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            var web = CreateTestSettingSite(services =>
            {
                var fakeGameRound = Substitute.For<IGameRound>();

                _ = fakeGameRound.ExistAsync(roundId).Returns(ValueTask.FromResult(true));

                _ = fakeGameRound.UpdateRoundReopenAsync(new UpdateRoundReopen(
                    RoundId: roundId,
                    Status: model.Status,
                    ExternalClosedOn: model.ExternalClosedOn,
                    BetAmount: model.BetAmount,
                    ValidBetAmount: model.ValidBetAmount,
                    SettleAmount: model.SettleAmount,
                    Commission: model.Commission,
                    Turnover: model.Turnover,
                    WinLoss: model.WinLoss,
                    ClientStatus: model.ClientStatus,
                    AdditionalData: model.AdditionalData));

                var gameRound = new GameRoundInfo(
                    RoundId: roundId,
                    ExternalRoundId: externalRoundId,
                    GameProviderId: gameProviderId,
                    PlayerCode: playerCode,
                    GameId: gameId,
                    CurrencyCode: "CNY",
                    Status: GameRoundStatus.Closed,
                    GameProviderConfigurationId: Guid.NewGuid(),
                    WalletId: Guid.NewGuid(),
                    CreatedOn: DateTimeOffset.UtcNow,
                    ClosedOn: null,
                    UpdatedOn: DateTimeOffset.UtcNow,
                    BrandId: Guid.NewGuid(),
                    ExternalCreatedOn: DateTimeOffset.UtcNow,
                    ExternalClosedOn: null,
                    BetAmount: model.BetAmount,
                    ValidBetAmount: model.ValidBetAmount,
                    SettleAmount: model.SettleAmount,
                    Commission: model.Commission,
                    Turnover: model.Turnover,
                    WinLoss: model.WinLoss,
                    TieBetAdjustment: null,
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

            await sut.UpdateRoundReopenAsync(roundId, model).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Any<GameRoundInfo>());
        }
    }
}
