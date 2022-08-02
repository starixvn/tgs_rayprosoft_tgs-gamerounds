using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories.UnitTest
{
    [TestClass]
    public class GameRoundNewRoundUnitTest
    {
        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeLogger = NullLogger<GameRoundRepository>.Instance;
            var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
            var fakeGameRound = Substitute.For<IGameRound>();
            var options = new DbContextOptionsBuilder<BetContext>()
               .UseInMemoryDatabase(databaseName: "bet")
               .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;
            var context = new BetContext(options);
            _ = context.Database.EnsureDeleted();

            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var expected = Guid.NewGuid();
            var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");
            var data = new NewRound(
                ExternalRoundId: externalRoundId,
                GameProviderId: gameProviderId,
                PlayerCode: playerCode,
                GameId: gameId,
                CurrencyCode: "CNY",
                GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId: walletId,
                BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn: DateTimeOffset.UtcNow);

            var fakeRoundRepository = new GameRoundRepository(
                context,
                fakeLogger,
                fakeSerialNumberGenerator);

            _ = fakeSerialNumberGenerator.GenerateAsync().Returns(ValueTask.FromResult(expected));

            _ = await fakeRoundRepository.NewRoundAsync(data).ConfigureAwait(false);

            var actual = await fakeRoundRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.RoundId);
            Assert.AreEqual(data.ExternalRoundId, actual.ExternalRoundId);
            Assert.AreEqual(data.GameProviderId, actual.GameProviderId);
            Assert.AreEqual(data.PlayerCode, actual.PlayerCode);
            Assert.AreEqual(data.GameId, actual.GameId);
            Assert.AreEqual(data.WalletId, actual.WalletId);
            Assert.AreEqual(GameRoundStatus.New, actual.Status);
            Assert.AreEqual(GameRoundClientStatus.Open, actual.ClientStatus);
            Assert.IsNotNull(actual.CreatedOn);
            Assert.IsNotNull(actual.UpdatedOn);
            Assert.AreEqual(data.ExternalCreatedOn, actual.ExternalCreatedOn);
            Assert.IsNull(actual.ClosedOn);
            Assert.IsNull(actual.ExternalClosedOn);
            Assert.IsNull(actual.BetAmount);
            Assert.IsNull(actual.ValidBetAmount);
            Assert.IsNull(actual.SettleAmount);
            Assert.IsNull(actual.Commission);
            Assert.IsNull(actual.Turnover);
            Assert.IsNull(actual.CompanyWinLoss);
        }
    }
}
