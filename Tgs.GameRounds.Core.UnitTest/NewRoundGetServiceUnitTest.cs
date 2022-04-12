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

            var query = new NewRoundQuery(
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
                           ExternalRoundId: query.ExternalRoundId,
                           GameProviderId: query.GameProviderId,
                           PlayerCode: query.PlayerCode,
                           GameId: query.GameId,
                           CurrencyCode: query.CurrencyCode,
                           Status: GameRoundStatus.Open,
                           GameProviderConfigurationId: query.GameProviderConfigurationId,
                           WalletId: query.WalletId,
                           CreatedOn: DateTimeOffset.UtcNow,
                           ClosedOn: null,
                           UpdatedOn: DateTimeOffset.UtcNow,
                           BrandId: query.BrandId,
                           ExternalCreatedOn: query.ExternalCreatedOn,
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

            _ = fakeGameRoundCache.GetRoundIdAsync(
                query.GameProviderId,
                query.ExternalRoundId,
                query.GameId,
                query.WalletId).Returns(ValueTask.FromResult<Guid?>(expected));

            var service = new NewRoundGetService(
                fakeGameRoundCache,
                fakeGameRoundRecord,
                fakeGameRound);

            var actual = await service.GetAsync(query).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);

            _ = fakeGameRoundRecord.Received(0).AddAsync(Arg.Is(gameRound));
        }

        [TestMethod]
        public async Task GameRound_Failed_To_Create_An_Order()
        {
            var fakeGameRoundCache = Substitute.For<IGameRoundService>();
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRoundRepository>();

            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";

            var query = new NewRoundQuery(
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

            _ = fakeGameRoundCache.LockAsync(
                query.GameProviderId,
                query.ExternalRoundId,
                query.GameId,
                query.WalletId).Returns(true);

            _ = fakeGameRound.NewRoundAsync(
                new(query.ExternalRoundId,
                query.GameProviderId,
                query.PlayerCode,
                query.GameId,
                query.CurrencyCode,
                query.GameProviderConfigurationId,
                query.WalletId,
                query.BrandId,
                query.ExternalCreatedOn)).Returns(ValueTask.FromResult<Guid?>(null));

            var service = new NewRoundGetService(
                fakeGameRoundCache,
                fakeGameRoundRecord,
                fakeGameRound);

            _ = await Assert.ThrowsExceptionAsync<Exception>(() => service.GetAsync(query).AsTask()).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(0).AddAsync(Arg.Any<GameRoundInfo>());

            _ = fakeGameRoundCache
                .Received(1)
                .UnLockAsync(
                    query.GameProviderId,
                    query.ExternalRoundId,
                    query.GameId,
                    query.WalletId);
        }

        [TestMethod]
        public async Task GameRound_Generated_Order_Is_Currently_Locked()
        {
            var fakeGameRoundCache = Substitute.For<IGameRoundService>();
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRoundRepository>();

            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";

            var query = new NewRoundQuery(
                ExternalRoundId: externalRoundId,
                GameProviderId: gameProviderId,
                PlayerCode: playerCode,
                GameId: gameId,
                CurrencyCode: "CNY",
                GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId: new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
                BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn: DateTimeOffset.UtcNow);

            _ = fakeGameRoundCache.LockAsync(
                query.GameProviderId,
                query.ExternalRoundId,
                query.GameId,
                query.WalletId).Returns(false);

            var service = new NewRoundGetService(
                fakeGameRoundCache,
                fakeGameRoundRecord,
                fakeGameRound);

            _ = await Assert.ThrowsExceptionAsync<Exception>(() => service.GetAsync(query).AsTask()).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(0).AddAsync(Arg.Any<GameRoundInfo>());
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

            var query = new NewRoundQuery(
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
                ExternalRoundId: query.ExternalRoundId,
                GameProviderId: query.GameProviderId,
                PlayerCode: query.PlayerCode,
                GameId: query.GameId,
                CurrencyCode: query.CurrencyCode,
                Status: GameRoundStatus.Open,
                GameProviderConfigurationId: query.GameProviderConfigurationId,
                WalletId: query.WalletId,
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: null,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: query.BrandId,
                ExternalCreatedOn: query.ExternalCreatedOn,
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

            var service = new NewRoundGetService(
                fakeGameRoundCache,
                fakeGameRoundRecord,
                fakeGameRound);

            _ = fakeGameRoundCache.LockAsync(
                query.GameProviderId,
                query.ExternalRoundId,
                query.GameId,
                query.WalletId).Returns(true);

            _ = fakeGameRound.NewRoundAsync(
                new(query.ExternalRoundId,
                query.GameProviderId,
                query.PlayerCode,
                query.GameId,
                query.CurrencyCode,
                query.GameProviderConfigurationId,
                query.WalletId,
                query.BrandId,
                query.ExternalCreatedOn)).Returns(ValueTask.FromResult<Guid?>(expected));
            _ = fakeGameRound.GetAsync(expected).Returns(ValueTask.FromResult(gameRound));

            var actual = await service.GetAsync(query).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Is(gameRound));

            _ = fakeGameRoundCache
                .Received(1)
                .UnLockAsync(
                    query.GameProviderId,
                    query.ExternalRoundId,
                    query.GameId,
                    query.WalletId);
        }
    }
}
