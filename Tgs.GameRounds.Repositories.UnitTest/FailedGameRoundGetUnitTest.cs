using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Repositories;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameProviders.GameRecord.DataBaseServer.Repositories.UnitTest;

[TestClass]
public class FailedGameRoundGetUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        var options = new DbContextOptionsBuilder<BetContext>()
           .UseInMemoryDatabase(databaseName: "bet")
           .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
           .Options;
        var context = new BetContext(options);
        _ = context.Database.EnsureDeleted();
        var gameProviderId = Guid.NewGuid();
        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameId = 0;
        var playerCode = "test";
        var failed = new FailedGameRecordOperation()
        {
            Id = Guid.NewGuid(),
            GameProviderId = gameProviderId,
            GameProviderCode = "tcg",
            ExternalRoundId = externalRoundId,
            PlayerCode = playerCode,
            GameId = gameId,
            ExternalId = "tcg_ellote",
            Detail = "{\"amount\":1,\"ref_no\":\"5621252\",\"username\":\"4uei1oy5zkkr\",\"additionalInformation\":{\"order_no\":\"TI09241097SHOV2IA\"}}",
            AutoStatus = (short)FailedGameRoundAutoStatus.Auto,
            ProcesStatus = (short)FailedGameRoundProcesStatus.Process,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        _ = await context.FailedGameRecordOperations.AddAsync(failed).ConfigureAwait(false);
        _ = await context.SaveChangesAsync().ConfigureAwait(false);

        var fakeRepository = new FailedGameRoundRepository(
            fakeSerialNumberGenerator,
            context);

        var actual = await fakeRepository.GetAsync(gameProviderId, externalRoundId, gameId, playerCode).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(failed.Id, actual.FailedGameRoundId);
    }
}
