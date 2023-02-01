using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;

namespace Tgs.GameRounds.Core.UnitTest;

[TestClass]
public class UpdateRoundDetailCommandServiceUnitTest
{
    [TestMethod]
    public async Task ExternalRoundId_Not_Exist()
    {
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var fakeLogger = NullLogger<UpdateRoundDetailCommandService>.Instance;

        var roundId = Guid.NewGuid();
        var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
        var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

        var command = new UpdateRoundDetailCommand(
            RoundId: roundId,
            AdditionalData: "{\"id\":10531704494,\"tableId\":10102,\"shoeId\":17,\"playId\":34,\"lobbyId\":1,\"gameType\":1,\"gameId\":1,\"memberId\":73304209,\"parentId\":19377,\"betTime\":\"2022-03-1709:19:38\",\"calTime\":\"2022-03-1709:20:01\",\"winOrLoss\":40.0,\"balanceBefore\":6020.41,\"betPoints\":20.0,\"betPointsz\":0.0,\"availableBet\":20.0,\"userName\":\"4UDW8YGX311V\",\"result\":\"{\\u0022result\\u0022:\\u00225,2,7\\u0022,\\u0022poker\\u0022:{\\u0022banker\\u0022:\\u002238-36-36\\u0022,\\u0022player\\u0022:\\u002246-36-0\\u0022}}\",\"betDetail\":\"{\\u0022player\\u0022:20.0,\\u0022playerW\\u0022:40.0}\",\"ip\":\"220.132.206.70\",\"ext\":\"220317B020851\",\"isRevocation\":1,\"currencyId\":1,\"deviceType\":5,\"serial\":\"-10531704494\",\"roadid\":0,\"pluginid\":0}"
            );

        _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult(false));

        var service = new UpdateRoundDetailCommandService(
            fakeLogger,
            fakeGameRound);

        _ = await Assert.ThrowsExceptionAsync<GameRoundDoesNotExistException>(() => service.ExecuteAsync(command).AsTask()).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var fakeLogger = NullLogger<UpdateRoundDetailCommandService>.Instance;

        var roundId = Guid.NewGuid();

        var command = new UpdateRoundDetailCommand(
            RoundId: roundId,
            AdditionalData: "{\"id\":10531704494,\"tableId\":10102,\"shoeId\":17,\"playId\":34,\"lobbyId\":1,\"gameType\":1,\"gameId\":1,\"memberId\":73304209,\"parentId\":19377,\"betTime\":\"2022-03-1709:19:38\",\"calTime\":\"2022-03-1709:20:01\",\"winOrLoss\":40.0,\"balanceBefore\":6020.41,\"betPoints\":20.0,\"betPointsz\":0.0,\"availableBet\":20.0,\"userName\":\"4UDW8YGX311V\",\"result\":\"{\\u0022result\\u0022:\\u00225,2,7\\u0022,\\u0022poker\\u0022:{\\u0022banker\\u0022:\\u002238-36-36\\u0022,\\u0022player\\u0022:\\u002246-36-0\\u0022}}\",\"betDetail\":\"{\\u0022player\\u0022:20.0,\\u0022playerW\\u0022:40.0}\",\"ip\":\"220.132.206.70\",\"ext\":\"220317B020851\",\"isRevocation\":1,\"currencyId\":1,\"deviceType\":5,\"serial\":\"-10531704494\",\"roadid\":0,\"pluginid\":0}"
            );

        _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult(true));

        _ = fakeGameRound.UpdateRoundDetailAsync(command.RoundId, command.AdditionalData);

        var service = new UpdateRoundDetailCommandService(
            fakeLogger,
            fakeGameRound);

        await service.ExecuteAsync(command).ConfigureAwait(false);
    }
}
