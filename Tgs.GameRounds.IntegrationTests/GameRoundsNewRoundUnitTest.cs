using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.IntegrationTests
{
    public partial class GameRoundsClientUnitTest
    {
        [TestMethod]
        public async Task GameRounds_NewRound_GameRound_Failed_To_Create_An_Order()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var model = new NewRoundQueryViewModel()
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

            var web = CreateTestSettingSite(services =>
            {
                var fakeService = Substitute.For<IGetService<NewRoundQuery, Guid>>();

                _ = fakeService.GetAsync(Arg.Any<NewRoundQuery>()).Throws<GameRoundCreationFailedException>();

                _ = services.AddSingleton(fakeService);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.NewRoundAsync(model).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual.Success);
            Assert.AreEqual(HttpStatusCode.InternalServerError, actual.HttpStatusCode);
            Assert.IsNull(actual.RoundId);
        }

        [TestMethod]
        public async Task GameRounds_NewRound_GameRound_Generated_Order_Is_Currently_Locked()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var model = new NewRoundQueryViewModel()
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

            var web = CreateTestSettingSite(services =>
            {
                var fakeGameRound = Substitute.For<IGameRound>();

                _ = fakeGameRound.GetRoundIdAsync(model.GameProviderId, model.ExternalRoundId, model.GameId, model.WalletId).Returns(ValueTask.FromResult<Guid?>(null));

                _ = services.AddSingleton(fakeGameRound);

                var fakeGameRoundLocked = Substitute.For<IGameRoundLocked>();

                _ = fakeGameRoundLocked.LockingAsync(
                    gameProviderId: model.GameProviderId,
                    externalRoundId: model.ExternalRoundId,
                    gameId: model.GameId,
                    walletId: model.WalletId).Returns(false);

                _ = services.AddSingleton(fakeGameRoundLocked);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.NewRoundAsync(model).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual.Success);
            Assert.AreEqual(HttpStatusCode.InternalServerError, actual.HttpStatusCode);
            Assert.IsNull(actual.RoundId);
        }

        [TestMethod]
        public async Task GameRounds_NewRound_Normal_Process()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var expected = Guid.NewGuid();
            var model = new NewRoundQueryViewModel()
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

            var web = CreateTestSettingSite(services =>
            {
                var fakeService = Substitute.For<IGetService<NewRoundQuery, Guid>>();

                _ = fakeService.GetAsync(Arg.Any<NewRoundQuery>()).Returns(expected);

                _ = services.AddSingleton(fakeService);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.NewRoundAsync(model).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(HttpStatusCode.OK, actual.HttpStatusCode);
            Assert.AreEqual(expected, actual.RoundId);
        }
    }
}
