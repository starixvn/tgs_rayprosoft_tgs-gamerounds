using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.UnitTest;

[TestClass]
public class UpdateRoundOpenCommandServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameRound = Substitute.For<IGameRoundRepository>();

        var roundId = Guid.NewGuid();

        var command = new UpdateRoundOpenCommand(
            RoundId: roundId,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            Commission: 0,
            Turnover: 10m);

        var service = new UpdateRoundOpenCommandService(
            fakeGameRound);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        var updateRoundOpen = new UpdateRoundOpen(
            BetAmount: command.BetAmount,
            ValidBetAmount: command.ValidBetAmount,
            Commission: command.Commission,
            Turnover: command.Turnover);

        _ = fakeGameRound
            .Received(1)
            .UpdateRoundOpenAsync(
                Arg.Is(command.RoundId),
                Arg.Is(updateRoundOpen),
                Arg.Any<CancellationToken>());
    }
}
