using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;

namespace Tgs.GameRounds.Core.UnitTest
{
    [TestClass]
    public class NewRoundGetServiceUnitTest
    {
        [TestMethod]
        public async Task ExternalRoundId_Exist()
        {
            var fakeGameRoundCache = Substitute.For<IGameRoundService>();
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRoundRepository>();

            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";

            var data = new NewRoundQuery(
                ExternalRoundId: externalRoundId,
                GameProviderId: gameProviderId,
                PlayerCode: playerCode,
                GameId: gameId,
                CurrencyCode: "CNY",
                GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId: new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
                BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn: DateTimeOffset.UtcNow);

            var expected = Guid.NewGuid();

            var gameRound = new GameRoundInfo(
                           RoundId: expected,
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
                           AdditionalData: string.Empty
                           );

            var repository = new NewRoundGetService(
                fakeGameRoundCache,
                fakeGameRoundRecord,
                fakeGameRound);

            _ = fakeGameRoundCache.GetRoundIdAsync(
                data.GameProviderId,
                data.ExternalRoundId,
                data.GameId,
                data.WalletId).Returns(ValueTask.FromResult<Guid?>(expected));

            var actual = await repository.GetAsync(data).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(expected, actual.RoundId);

            _ = fakeGameRoundRecord.Received(0).AddAsync(Arg.Is(gameRound));
        }

        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameRoundCache = Substitute.For<IGameRoundService>();
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRoundRepository>();

            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";

            var data = new NewRoundQuery(
                ExternalRoundId: externalRoundId,
                GameProviderId: gameProviderId,
                PlayerCode: playerCode,
                GameId: gameId,
                CurrencyCode: "CNY",
                GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId: new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
                BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn: DateTimeOffset.UtcNow);

            var expected = Guid.NewGuid();

            var gameRound = new GameRoundInfo(
                RoundId: expected,
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
                AdditionalData: string.Empty
                );

            var repository = new NewRoundGetService(
                fakeGameRoundCache,
                fakeGameRoundRecord,
                fakeGameRound);

            _ = fakeGameRound.NewRoundAsync(
                new(data.ExternalRoundId,
                data.GameProviderId,
                data.PlayerCode,
                data.GameId,
                data.CurrencyCode,
                data.GameProviderConfigurationId,
                data.WalletId,
                data.BrandId,
                data.ExternalCreatedOn)).Returns(ValueTask.FromResult<Guid?>(expected));
            _ = fakeGameRound.GetAsync(expected).Returns(ValueTask.FromResult(gameRound));

            var actual = await repository.GetAsync(data).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(expected, actual.RoundId);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Is(gameRound));
        }
    }
}
