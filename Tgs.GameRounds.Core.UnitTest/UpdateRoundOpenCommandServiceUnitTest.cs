using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.UnitTest
{
    [TestClass]
    public class UpdateRoundOpenCommandServiceUnitTest
    {
        [TestMethod]
        public async Task ExternalRoundId_Not_Exist()
        {
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();
            var fakeGameRoundConvert = Substitute.For<IGameRoundConvert>();
            var fakeLogger = NullLogger<UpdateRoundOpenCommandService>.Instance;

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

            var command = new UpdateRoundOpenCommand(
                RoundId: roundId,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                Commission: 0,
                Turnover: 10m);

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult(false));

            var service = new UpdateRoundOpenCommandService(
                fakeLogger,
                fakeGameRound,
                fakeGameRoundRecordMessage,
                fakeGameRoundConvert);

            _ = await Assert.ThrowsExceptionAsync<GameRoundDoesNotExistException>(() => service.ExecuteAsync(command).AsTask()).ConfigureAwait(false);

            _ = fakeGameRoundRecordMessage
                .Received(0)
                .SenderGameRoundRecord(Arg.Is(command.RoundId));
        }

        [TestMethod]
        public async Task Game_Provider_Currency_IDRK_Is_Converted_Into_The_Platform_Currency_IDR()
        {
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();
            var fakeGameRoundConvert = Substitute.For<IGameRoundConvert>();
            var fakeLogger = NullLogger<UpdateRoundOpenCommandService>.Instance;

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var currencyCode = "CNY";

            var command = new UpdateRoundOpenCommand(
                RoundId: roundId,
                BetAmount: 1m,
                ValidBetAmount: 1m,
                Commission: 0,
                Turnover: 1m);

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult<bool>(true));

            _ = fakeGameRound
                .GetByCurrencyInfoAsync(command.RoundId)
                .Returns(ValueTask.FromResult<(Guid GameProviderId, string CurrencyCode)>(new(gameProviderId, currencyCode)));

            var gameRoundConvertRequest = new GameRoundConvertRequest(
                GameProviderId: gameProviderId,
                CurrencyCode: currencyCode,
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: 0,
                Commission: command.Commission,
                Turnover: command.Turnover,
                PlayerWinLoss: 0);
            var gameRoundConvertResult = new GameRoundConvertResult(
                BetAmount: 1000m,
                ValidBetAmount: 1000m,
                SettleAmount: 0,
                Commission: 0m,
                Turnover: 1000m,
                PlayerWinLoss: 0);
            _ = fakeGameRoundConvert.ConvertPlatformRoundsAsync(
                request: gameRoundConvertRequest).Returns(gameRoundConvertResult);

            var updateRoundOpen = new UpdateRoundOpen(
                RoundId: command.RoundId,
                BetAmount: gameRoundConvertResult.BetAmount,
                ValidBetAmount: gameRoundConvertResult.ValidBetAmount,
                Commission: gameRoundConvertResult.Commission,
                Turnover: gameRoundConvertResult.Turnover);
            _ = fakeGameRound.UpdateRoundOpenAsync(updateRoundOpen);

            var service = new UpdateRoundOpenCommandService(
                fakeLogger,
                fakeGameRound,
                fakeGameRoundRecordMessage,
                fakeGameRoundConvert);

            await service.ExecuteAsync(command).ConfigureAwait(false);

            _ = fakeGameRoundRecordMessage
                .Received(1)
                .SenderGameRoundRecord(Arg.Is(command.RoundId));
        }

        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();
            var fakeGameRoundConvert = Substitute.For<IGameRoundConvert>();
            var fakeLogger = NullLogger<UpdateRoundOpenCommandService>.Instance;

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var currencyCode = "CNY";

            var command = new UpdateRoundOpenCommand(
                RoundId: roundId,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                Commission: 0,
                Turnover: 10m);

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult<bool>(true));

            _ = fakeGameRound
                .GetByCurrencyInfoAsync(command.RoundId)
                .Returns(ValueTask.FromResult<(Guid GameProviderId, string CurrencyCode)>(new(gameProviderId, currencyCode)));

            var gameRoundConvertRequest = new GameRoundConvertRequest(
                GameProviderId: gameProviderId,
                CurrencyCode: currencyCode,
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: 0,
                Commission: command.Commission,
                Turnover: command.Turnover,
                PlayerWinLoss: 0);
            var gameRoundConvertResult = new GameRoundConvertResult(
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: 0,
                Commission: command.Commission,
                Turnover: command.Turnover,
                PlayerWinLoss: 0);
            _ = fakeGameRoundConvert.ConvertPlatformRoundsAsync(
                request: gameRoundConvertRequest).Returns(gameRoundConvertResult);

            var updateRoundOpen = new UpdateRoundOpen(
                RoundId: command.RoundId,
                BetAmount: gameRoundConvertResult.BetAmount,
                ValidBetAmount: gameRoundConvertResult.ValidBetAmount,
                Commission: gameRoundConvertResult.Commission,
                Turnover: gameRoundConvertResult.Turnover);
            _ = fakeGameRound.UpdateRoundOpenAsync(updateRoundOpen);

            var service = new UpdateRoundOpenCommandService(
                fakeLogger,
                fakeGameRound,
                fakeGameRoundRecordMessage,
                fakeGameRoundConvert);

            await service.ExecuteAsync(command).ConfigureAwait(false);

            _ = fakeGameRoundRecordMessage
                .Received(1)
                .SenderGameRoundRecord(Arg.Is(command.RoundId));
        }
    }
}
