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
public class UpdateRoundOpenSettleCommandServiceUnitTest
{
    [TestMethod]
    public async Task ExternalRoundId_Not_Exist()
    {
        var fakeGameRound = Substitute.For<IGameRound>();
        var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();
        var fakeLogger = NullLogger<UpdateRoundOpenSettleCommandService>.Instance;

        var roundId = Guid.NewGuid();
        var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
        var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

        var command = new UpdateRoundOpenSettleCommand(
            RoundId: roundId,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: 10m);

        _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult(false));

        var service = new UpdateRoundOpenSettleCommandService(
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
        var fakeLogger = NullLogger<UpdateRoundOpenSettleCommandService>.Instance;

        var roundId = Guid.NewGuid();

        var command = new UpdateRoundOpenSettleCommand(
            RoundId: roundId,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: 10m);

        _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult<bool>(true));

        var updateRoundOpenSettle = new UpdateRoundOpenSettle(
            BetAmount: command.BetAmount,
            ValidBetAmount: command.ValidBetAmount,
            SettleAmount: command.SettleAmount,
            Commission: command.Commission,
            Turnover: command.Turnover,
            PlayerWinLoss: command.PlayerWinLoss);
        _ = fakeGameRound.UpdateRoundOpenSettleAsync(roundId, updateRoundOpenSettle);

        var service = new UpdateRoundOpenSettleCommandService(
            fakeLogger,
            fakeGameRound,
            fakeGameRoundRecordMessage);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameRoundRecordMessage
            .Received(1)
            .SenderGameRoundRecord(Arg.Is(command.RoundId));
    }
}
