using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories.UnitTest
{
    [TestClass]
    public class UpdateClientStatusUnitTest
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
            var roundId = Guid.NewGuid();

            var round = new Round()
            {
                Id = roundId,
                ExternalRoundId = externalRoundId,
                GameProviderId = gameProviderId,
                PlayerCode = playerCode,
                GameId = gameId,
                CurrencyCode = "CNY",
                Status = (short)GameRoundStatus.Open,
                GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
                BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                CreatedOn = DateTime.UtcNow,
                ExternalCreatedOn = DateTime.UtcNow,
                BetAmount = 20m,
                ValidBetAmount = 20m,
                SettleAmount = 40m,
                Commission = 0m,
                Turnover = 20m,
                WinLoss = -20m,
                ClientStatus = (short)GameRoundClientStatus.Open
            };

            _ = await context.AddAsync(round).ConfigureAwait(false);
            _ = await context.SaveChangesAsync().ConfigureAwait(false);

            var fakeRoundRepository = new GameRoundRepository(
                context,
                fakeGameRound,
                fakeLogger,
                fakeSerialNumberGenerator);

            await fakeRoundRepository.UpdateClientStatusAsync(roundId, GameRoundClientStatus.Win).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Round_Did_Not_Exist()
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

            var roundId = Guid.NewGuid();

            var fakeRoundRepository = new GameRoundRepository(
                context,
                fakeGameRound,
                fakeLogger,
                fakeSerialNumberGenerator);

            _ = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => fakeRoundRepository.UpdateClientStatusAsync(
                roundId,
                GameRoundClientStatus.Win).AsTask()).ConfigureAwait(false);
        }
    }
}
