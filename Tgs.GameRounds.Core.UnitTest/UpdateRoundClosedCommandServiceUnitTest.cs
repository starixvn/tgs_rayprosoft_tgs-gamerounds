using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.UnitTest;

[TestClass]
public class UpdateRoundClosedCommandServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();
        var fakeLogger = NullLogger<UpdateRoundClosedCommandService>.Instance;

        var roundId = Guid.NewGuid();

        var command = new UpdateRoundClosedCommand(
            RoundId: roundId,
            ExternalClosedOn: DateTimeOffset.UtcNow,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 0,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: -10m,
            ClientStatus: GameRoundClientStatus.Loss,
            AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

        var updateRoundClosed = new UpdateRoundClosed(
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
            .UpdateRoundClosedAsync(
                command.RoundId,
                updateRoundClosed);

        var service = new UpdateRoundClosedCommandService(
            fakeLogger,
            fakeGameRound,
            fakeGameRoundRecordMessage);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameRoundRecordMessage
            .Received(1)
            .SenderGameRoundRecord(Arg.Is(command.RoundId));
    }
}
