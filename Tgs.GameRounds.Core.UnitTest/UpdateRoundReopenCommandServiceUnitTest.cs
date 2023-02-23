using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.UnitTest;

[TestClass]
public class UpdateRoundReopenCommandServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameRound = Substitute.For<IGameRoundRepository>();

        var roundId = Guid.NewGuid();

        var command = new UpdateRoundReopenCommand(
            RoundId: roundId,
            Status: GameRoundStatus.Closed,
            ExternalClosedOn: DateTimeOffset.UtcNow,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: 10m,
            ClientStatus: GameRoundClientStatus.Win,
            AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

        var service = new UpdateRoundReopenCommandService(
            fakeGameRound);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        var updateRoundReopen = new UpdateRoundReopen(
            Status: command.Status,
            ExternalClosedOn: command.ExternalClosedOn,
            BetAmount: command.BetAmount,
            ValidBetAmount: command.ValidBetAmount,
            SettleAmount: command.SettleAmount,
            Commission: command.Commission,
            Turnover: command.Turnover,
            PlayerWinLoss: command.PlayerWinLoss,
            ClientStatus: command.ClientStatus,
            AdditionalData: command.AdditionalData);

        _ = fakeGameRound
            .Received(1)
            .UpdateRoundReopenAsync(
                Arg.Is(command.RoundId),
                Arg.Is(updateRoundReopen),
                Arg.Any<CancellationToken>());
    }
}
