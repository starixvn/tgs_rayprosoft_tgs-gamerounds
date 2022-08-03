using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;

namespace Tgs.GameRounds.Core.UnitTest
{
    [TestClass]
    public class NewRoundGetServiceUnitTest
    {
        [TestMethod]
        public async Task ExternalRoundId_Exist()
        {
            var fakeLogger = NullLogger<NewRoundGetService>.Instance;
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundLocked = Substitute.For<IGameRoundLocked>();
            var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();

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

            _ = fakeGameRound.GetRoundIdAsync(
                query.GameProviderId,
                query.ExternalRoundId,
                query.GameId,
                query.WalletId).Returns(ValueTask.FromResult<Guid?>(expected));

            var service = new NewRoundGetService(
                fakeLogger,
                fakeGameRound,
                fakeGameRoundLocked,
                fakeGameRoundRecordMessage);

            var actual = await service.GetAsync(query).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);

            _ = fakeGameRoundRecordMessage
                .Received(0)
                .SenderGameRoundRecord(Arg.Is(expected));
        }

        [TestMethod]
        public async Task GameRound_Failed_To_Create_An_Order()
        {
            var fakeLogger = NullLogger<NewRoundGetService>.Instance;
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundLocked = Substitute.For<IGameRoundLocked>();
            var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();

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

            _ = fakeGameRoundLocked.LockingAsync(
                Arg.Is(query.GameProviderId),
                Arg.Is(query.ExternalRoundId),
                Arg.Is(query.GameId),
                Arg.Is(query.WalletId)).Returns(true);

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
                fakeLogger,
                fakeGameRound,
                fakeGameRoundLocked,
                fakeGameRoundRecordMessage);

            _ = await Assert.ThrowsExceptionAsync<GameRoundCreationFailedException>(() => service.GetAsync(query).AsTask()).ConfigureAwait(false);

            _ = fakeGameRoundRecordMessage
                .Received(0)
                .SenderGameRoundRecord(Arg.Any<Guid>());

            _ = fakeGameRoundLocked.Received(1).UnlockAsync(
                Arg.Is(query.GameProviderId),
                Arg.Is(query.ExternalRoundId),
                Arg.Is(query.GameId),
                Arg.Is(query.WalletId));
        }

        [TestMethod]
        public async Task GameRound_Generated_Order_Is_Currently_Locked()
        {
            var fakeLogger = NullLogger<NewRoundGetService>.Instance;
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundLocked = Substitute.For<IGameRoundLocked>();
            var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();

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

            _ = fakeGameRoundLocked.LockingAsync(
                Arg.Is(query.GameProviderId),
                Arg.Is(query.ExternalRoundId),
                Arg.Is(query.GameId),
                Arg.Is(query.WalletId)).Returns(false);

            var service = new NewRoundGetService(
                fakeLogger,
                fakeGameRound,
                fakeGameRoundLocked,
                fakeGameRoundRecordMessage);

            _ = await Assert.ThrowsExceptionAsync<GameRoundCreationFailedException>(() => service.GetAsync(query).AsTask()).ConfigureAwait(false);

            _ = fakeGameRoundRecordMessage
                .Received(0)
                .SenderGameRoundRecord(Arg.Any<Guid>());

            _ = fakeGameRoundLocked.Received(0).UnlockAsync(
                Arg.Is(query.GameProviderId),
                Arg.Is(query.ExternalRoundId),
                Arg.Is(query.GameId),
                Arg.Is(query.WalletId));
        }

        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeLogger = NullLogger<NewRoundGetService>.Instance;
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundLocked = Substitute.For<IGameRoundLocked>();
            var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();

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

            var service = new NewRoundGetService(
                fakeLogger,
                fakeGameRound,
                fakeGameRoundLocked,
                fakeGameRoundRecordMessage);

            _ = fakeGameRoundLocked.LockingAsync(
                Arg.Is(query.GameProviderId),
                Arg.Is(query.ExternalRoundId),
                Arg.Is(query.GameId),
                Arg.Is(query.WalletId)).Returns(true);

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

            var actual = await service.GetAsync(query).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);

            _ = fakeGameRoundRecordMessage
                .Received(1)
                .SenderGameRoundRecord(Arg.Is(expected));

            _ = fakeGameRoundLocked
                .Received(1)
                .UnlockAsync(
                    Arg.Is(query.GameProviderId),
                    Arg.Is(query.ExternalRoundId),
                    Arg.Is(query.GameId),
                    Arg.Is(query.WalletId));
        }
    }
}
