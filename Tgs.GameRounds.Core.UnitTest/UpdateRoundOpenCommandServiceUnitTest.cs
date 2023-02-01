using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.UnitTest;

[TestClass]
public class UpdateRoundOpenCommandServiceUnitTest
{

    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();
        var fakeLogger = NullLogger<UpdateRoundOpenCommandService>.Instance;

        var roundId = Guid.NewGuid();

        var command = new UpdateRoundOpenCommand(
            RoundId: roundId,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            Commission: 0,
            Turnover: 10m);

        var updateRoundOpen = new UpdateRoundOpen(
            BetAmount: command.BetAmount,
            ValidBetAmount: command.ValidBetAmount,
            Commission: command.Commission,
            Turnover: command.Turnover);
        _ = fakeGameRound.UpdateRoundOpenAsync(roundId, updateRoundOpen);

        var service = new UpdateRoundOpenCommandService(
            fakeLogger,
            fakeGameRound,
            fakeGameRoundRecordMessage);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameRoundRecordMessage
            .Received(1)
            .SenderGameRoundRecord(Arg.Is(command.RoundId));
    }
}
