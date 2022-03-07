using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.IntegrationTests
{
    [TestClass]
    public partial class GameActionsClientUnitTest
    {
        [TestMethod]
        public async Task GameRound_GetStatus()
        {
            var roundId = Guid.NewGuid();
            var expected = GameRoundStatus.Open;
            var data = new GameRoundInfo(
               RoundId: roundId,
               ExternalRoundId: Guid.NewGuid().ToString("N"),
               GameProviderId: Guid.NewGuid(),
               PlayerCode: "test",
               GameId: 1,
               CurrencyCode: "CNY",
               Status: expected,
               GameProviderConfigurationId: Guid.NewGuid(),
               WalletId: Guid.NewGuid(),
               CreatedOn: DateTimeOffset.UtcNow,
               ClosedOn: null,
               UpdatedOn: DateTimeOffset.UtcNow,
               BrandId: Guid.NewGuid(),
               ExternalCreatedOn: null,
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
               AdditionalData: string.Empty);

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeRepository = Substitute.For<IGameRoundRepository>();

                _ = fakeRepository.GetAsync(roundId).Returns(ValueTask.FromResult(data));

                _ = services.AddSingleton(fakeRepository);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.GameRoundGetStatus(roundId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task GameRounds_NewRound()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var expected = new NewRoundResultViewModel(true, Guid.NewGuid());
            var data = new NewRoundQueryViewModel()
            {
                ExternalRoundId = externalRoundId,
                GameProviderId = gameProviderId,
                PlayerCode = playerCode,
                GameId = gameId,
                CurrencyCode = "CNY",
                GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
                BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn = DateTimeOffset.UtcNow
            };

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeGameRoundCache = Substitute.For<IGameRoundCacheService>();
                _ = fakeGameRoundCache.GetRoundIdAsync(data.GameProviderId, data.ExternalRoundId, data.GameId, data.WalletId).Returns(ValueTask.FromResult<Guid?>(null));
                _ = services.AddSingleton(fakeGameRoundCache);

                var fakeGameRound = Substitute.For<IGameRoundRepository>();

                _ = fakeGameRound.NewRoundAsync(new(
                    data.ExternalRoundId,
                    data.GameProviderId,
                    data.PlayerCode,
                    data.GameId,
                    data.CurrencyCode,
                    data.GameProviderConfigurationId,
                    data.WalletId,
                    data.BrandId,
                    data.ExternalCreatedOn)).Returns(ValueTask.FromResult(expected.RoundId));

                var gameRound = new GameRoundInfo(
                    RoundId: expected.RoundId!.Value,
                   ExternalRoundId: data.ExternalRoundId,
                   GameProviderId: data.GameProviderId,
                   PlayerCode: data.PlayerCode,
                   GameId: data.GameId,
                   CurrencyCode: data.CurrencyCode,
                   Status: GameRoundStatus.Open,
                   GameProviderConfigurationId: data.GameProviderConfigurationId,
                   WalletId: data.WalletId,
                   CreatedOn: DateTimeOffset.UtcNow,
                   ClosedOn: null,
                   UpdatedOn: DateTimeOffset.UtcNow,
                   BrandId: data.BrandId,
                   ExternalCreatedOn: data.ExternalCreatedOn,
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
                   AdditionalData: string.Empty);

                _ = fakeGameRound.GetAsync(expected.RoundId.Value).Returns(ValueTask.FromResult(gameRound));

                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddSingleton(fakeGameRoundRecord);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.GameRoundsNewRoundAsync(data).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Equals(expected, actual);
            Assert.AreEqual(expected.Success, actual.Success);
            Assert.AreEqual(expected.RoundId, actual.RoundId);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Any<GameRoundInfo>());
        }

        [TestMethod]
        public async Task GameRounds_UpdateRound()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var data = new UpdateRoundQueryViewModel()
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

            var expected = new UpdateRoundResultViewModel(true);

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeGameRoundCache = Substitute.For<IGameRoundCacheService>();
                _ = fakeGameRoundCache.ExistAsync(roundId).Returns(ValueTask.FromResult(true));
                _ = services.AddSingleton(fakeGameRoundCache);

                var fakeGameRound = Substitute.For<IGameRoundRepository>();

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

                _ = fakeGameRound.GetAsync(roundId).Returns(ValueTask.FromResult(gameRound));

                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddSingleton(fakeGameRoundRecord);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.GameRoundsUpdateRoundAsync(roundId, data).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Equals(expected, actual);
            Assert.AreEqual(expected.Success, actual.Success);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Any<GameRoundInfo>());
        }

        private static WebApplicationFactory<Program> CreateTestSettingSite(Action<IServiceCollection>? configureServices = null)
            => new GameActionsApplicationUnitTest(hostBuilder =>
            {
                _ = hostBuilder
                .ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.Sources.Clear();
                })
                .ConfigureServices(services =>
                {
                    configureServices?.Invoke(services);
                    /*
                     var fake = Substitute.For<ISerialNumberGenerator>();
                     services.AddSingleton(fake);
                    */
                });
            });
    }
}
