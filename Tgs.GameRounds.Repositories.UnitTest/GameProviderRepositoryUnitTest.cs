using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories.UnitTest;

public class GameProviderRepositoryUnitTest
{
    [Fact]
    public async Task Get_Code()
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
        _ = await context.GameProviders.AddAsync(gameProvider);
        _ = await context.SaveChangesAsync();

        var fakeRoundRepository = new GameProviderRepository(context);

        var actual = await fakeRoundRepository.GetCode(gameProviderId);

        Assert.NotNull(actual);
        Assert.Equal(gameProviderCode, actual);
    }
}
