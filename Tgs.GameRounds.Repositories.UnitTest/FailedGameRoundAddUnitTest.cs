using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Repositories;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameProviders.GameRecord.DataBaseServer.Repositories.UnitTest;

[TestClass]
public class FailedGameRoundAddUnitTest
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

        var data = new AddFailedGameRound(
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "tcg",
            ExternalRoundId: Guid.NewGuid().ToString("N"),
            PlayerCode: "test",
            GameId: 0,
            ExternalGameId: "tcg_ELLOTE",
            Detail: "{\"amount\":1,\"ref_no\":\"5621252\",\"username\":\"4uei1oy5zkkr\",\"additionalInformation\":{\"order_no\":\"TI09241097SHOV2IA\"}}",
            AutoStatus: FailedGameRoundAutoStatus.Auto,
            ProcesStatus: FailedGameRoundProcesStatus.Process);

        var fakeRepository = new FailedGameRoundRepository(
            fakeSerialNumberGenerator,
            context);

        await fakeRepository.AddAsync(data).ConfigureAwait(false);
    }
}
