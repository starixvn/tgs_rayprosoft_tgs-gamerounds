using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.DataBase.Rounds.Core;
using Tgs.DataBase.Rounds.Core.Models;
using Tgs.DataBase.Rounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.DataBase.Rounds.Repositories.UnitTest
{
    [TestClass]
    public class VoidRoundUnitTest
    {
        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeLogger = NullLogger<RoundRepository>.Instance;
            var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
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
                Status = (short)RoundStatus.Open,
                BetAmount = 10m,
                ValidBetAmount = 10m,
                ClientStatus = (short)RoundClientStatus.Open
            };
            _ = await context.AddAsync(round).ConfigureAwait(false);
            _ = await context.SaveChangesAsync().ConfigureAwait(false);

            var data = new VoidRound(RoundId: roundId);

            var fakeRoundRepository = new RoundRepository(context, fakeLogger, fakeSerialNumberGenerator);

            await fakeRoundRepository.VoidRoundAsync(data).ConfigureAwait(false);

            var actual = await fakeRoundRepository.GetAsync(roundId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(RoundStatus.Closed, actual.Status);
            Assert.AreEqual(RoundClientStatus.Void, actual.ClientStatus);
        }
    }
}
