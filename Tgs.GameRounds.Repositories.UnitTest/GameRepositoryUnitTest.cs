using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories.UnitTest;

public class GameRepositoryUnitTest
{
    [Fact]
    public async Task Get_ExternalId()
    {
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

        var fakeRoundRepository = new GameRepository(context);

        var actual = await fakeRoundRepository.GetExternalIdAsync(gameId).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(externalGameId, actual);
    }

    [Fact]
    public async Task Get_Id()
    {
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

        var fakeRoundRepository = new GameRepository(context);

        var actual = await fakeRoundRepository.GetIdAsync(gameProviderId, externalGameId).ConfigureAwait(false);

        Assert.Equal(gameId, actual);
    }
}
