using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories.UnitTest;

[TestClass]
public class GameProviderRepositoryUnitTest
{
    [TestMethod]
    public async Task Get_GameProviderCode()
    {
        var options = new DbContextOptionsBuilder<TgsContext>()
           .UseInMemoryDatabase(databaseName: "dbo")
           .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
           .Options;
        var context = new TgsContext(options);
        _ = context.Database.EnsureDeleted();

        var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
        var gameProviderCode = "DG";
        var gameProvider = new GameProvider()
        {
            Id = gameProviderId,
            Status = 0,

            AutoCloseRound = true,
            SupportTestPlayer = false,
            AllowKickPlayer = true,
            PlaceBetTimeout = 1,
            SettleBetTimeout = 20,
            UniqueId = 25,
            TransferWallet = false,
            Code = gameProviderCode,
            Name = "DreamGame",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            CreatedByUsername = "admin",
            UpdatedByUsername = "admin"
        };
        _ = await context.GameProviders.AddAsync(gameProvider).ConfigureAwait(false);
        _ = await context.SaveChangesAsync().ConfigureAwait(false);

        var fakeRoundRepository = new GameProviderRepository(context);

        var actual = await fakeRoundRepository.GetGameProviderCodeAsync(gameProviderId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(gameProviderCode, actual);
    }
}
