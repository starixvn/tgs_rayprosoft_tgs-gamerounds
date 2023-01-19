using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;

namespace Tgs.GameRounds.Core.UnitTest;

[TestClass]
public class UpdateClientStatusCommandServiceUnitTest
{
    [TestMethod]
    public async Task ExternalRoundId_Not_Exist()
    {
        var fakeGameRound = Substitute.For<IGameRound>();
        var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();
        var fakeLogger = NullLogger<UpdateClientStatusCommandService>.Instance;

        var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
        var roundId = Guid.NewGuid();

        var command = new UpdateClientStatusCommand(
            RoundId: roundId,
            Status: GameRoundClientStatus.Loss);

        _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult(false));

        var service = new UpdateClientStatusCommandService(
            fakeLogger,
            fakeGameRound,
            fakeGameRoundRecordMessage);

        _ = await Assert.ThrowsExceptionAsync<GameRoundDoesNotExistException>(() => service.ExecuteAsync(command).AsTask()).ConfigureAwait(false);

        _ = fakeGameRoundRecordMessage
            .Received(0)
            .SenderGameRoundRecord(Arg.Is(command.RoundId));
    }

    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameRound = Substitute.For<IGameRound>();
        var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();
        var fakeLogger = NullLogger<UpdateClientStatusCommandService>.Instance;

        var roundId = Guid.NewGuid();

        var command = new UpdateClientStatusCommand(
            RoundId: roundId,
            Status: GameRoundClientStatus.Loss);

        _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult<bool>(true));

        _ = fakeGameRound.UpdateClientStatusAsync(
            command.RoundId,
            command.Status);

        var service = new UpdateClientStatusCommandService(
            fakeLogger,
            fakeGameRound,
            fakeGameRoundRecordMessage);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameRoundRecordMessage
            .Received(1)
            .SenderGameRoundRecord(Arg.Is(command.RoundId));
    }
}
