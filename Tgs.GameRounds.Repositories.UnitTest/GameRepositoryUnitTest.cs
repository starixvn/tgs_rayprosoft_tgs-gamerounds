using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories.UnitTest
{
    [TestClass]
    public class GameRepositoryUnitTest
    {
        [TestMethod]
        public async Task Get_GameId()
        {
            var fakeGame = Substitute.For<IGame>();
            var options = new DbContextOptionsBuilder<TgsContext>()
               .UseInMemoryDatabase(databaseName: "dbo")
               .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;
            var context = new TgsContext(options);
            _ = context.Database.EnsureDeleted();

            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalGameId = "Bacc";
            var gameId = 500001;
            var game = new Game()
            {
                Id = gameId,
                Status = 0,
                ExternalId = externalGameId,
                GameProviderId = gameProviderId,
                Type = 13,
                PlatformType = 3,
                SubGame = true,
                AvailableToBrand = false,
                HasTurnoverOnTieBet = true,
                Html5 = true,
                AvailableCurrencies = string.Empty,
                Code = "Bacc",
                Name = "Bacc",
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                CreatedByUsername = "test",
                UpdatedByUsername = "test"
            };
            _ = await context.Games.AddAsync(game).ConfigureAwait(false);
            _ = await context.SaveChangesAsync().ConfigureAwait(false);

            var fakeRoundRepository = new GameRepository(fakeGame, context);

            var actual = await fakeRoundRepository.GetGameIdAsync(gameProviderId, externalGameId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(gameId, actual);
        }

        [TestMethod]
        public async Task Get_ExternalGameId()
        {
            var fakeGame = Substitute.For<IGame>();
            var options = new DbContextOptionsBuilder<TgsContext>()
               .UseInMemoryDatabase(databaseName: "dbo")
               .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;
            var context = new TgsContext(options);
            _ = context.Database.EnsureDeleted();

            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalGameId = "Bacc";
            var gameId = 500001;
            var game = new Game()
            {
                Id = gameId,
                Status = 0,
                ExternalId = externalGameId,
                GameProviderId = gameProviderId,
                Type = 13,
                PlatformType = 3,
                SubGame = true,
                AvailableToBrand = false,
                HasTurnoverOnTieBet = true,
                Html5 = true,
                AvailableCurrencies = string.Empty,
                Code = "Bacc",
                Name = "Bacc",
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                CreatedByUsername = "test",
                UpdatedByUsername = "test"
            };
            _ = await context.Games.AddAsync(game).ConfigureAwait(false);
            _ = await context.SaveChangesAsync().ConfigureAwait(false);

            var fakeRoundRepository = new GameRepository(fakeGame, context);

            var actual = await fakeRoundRepository.GetExternalGameIdAsync(gameId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(externalGameId, actual);
        }
    }
}
