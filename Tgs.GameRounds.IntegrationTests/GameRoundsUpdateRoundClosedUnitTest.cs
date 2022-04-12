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

namespace Tgs.GameRounds.IntegrationTests
{
    public partial class GameRoundsClientUnitTest
    {
        [TestMethod]
        public async Task GameRounds_UpdateRoundClosed_Normal_Process()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var viewModel = new UpdateRoundClosedViewModel(
                ClosedOn: DateTimeOffset.UtcNow,
                UpdatedOn: DateTimeOffset.UtcNow,
                ExternalClosedOn: DateTimeOffset.UtcNow,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                SettleAmount: 0,
                Commission: 0,
                Turnover: 10m,
                WinLoss: -10m,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Loss,
                AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeGameRoundCache = Substitute.For<IGameRoundService>();
                _ = fakeGameRoundCache.ExistAsync(roundId).Returns(ValueTask.FromResult(true));
                _ = services.AddSingleton(fakeGameRoundCache);

                var fakeGameRound = Substitute.For<IGameRoundRepository>();

                _ = fakeGameRound.UpdateRoundClosedAsync(new(
                    roundId,
                    viewModel.ClosedOn,
                    viewModel.UpdatedOn,
                    viewModel.ExternalClosedOn,
                    viewModel.BetAmount,
                    viewModel.ValidBetAmount,
                    viewModel.SettleAmount,
                    viewModel.Commission,
                    viewModel.Turnover,
                    viewModel.WinLoss,
                    viewModel.TieBetAdjustment,
                    viewModel.ClientStatus,
                    viewModel.AdditionalData));

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
                    ExternalClosedOn: viewModel.ExternalClosedOn,
                    BetAmount: viewModel.BetAmount,
                    ValidBetAmount: viewModel.ValidBetAmount,
                    SettleAmount: viewModel.SettleAmount,
                    Commission: viewModel.Commission,
                    Turnover: viewModel.Turnover,
                    WinLoss: viewModel.WinLoss,
                    TieBetAdjustment: viewModel.TieBetAdjustment,
                    ClientStatus: GameRoundClientStatus.Open,
                    ReopenTurnoverAdjustment: null,
                    ReopenedOn: null,
                    EffectiveBettingPercentage: null,
                    AdditionalData: viewModel.AdditionalData ?? string.Empty);

                _ = fakeGameRound.GetAsync(roundId).Returns(ValueTask.FromResult(gameRound));

                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddSingleton(fakeGameRoundRecord);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            await sut.UpdateRoundClosedAsync(roundId, viewModel).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Any<GameRoundInfo>());
        }
    }
}
