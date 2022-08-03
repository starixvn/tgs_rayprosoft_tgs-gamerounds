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

namespace Tgs.GameRounds.Core.UnitTest
{
    [TestClass]
    public class UpdateRoundReopenCommandServiceUnitTest
    {
        [TestMethod]
        public async Task ExternalRoundId_Not_Exist()
        {
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();
            var fakeGameRoundConvert = Substitute.For<IGameRoundConvert>();
            var fakeLogger = NullLogger<UpdateRoundReopenCommandService>.Instance;

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

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

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult(false));

            var service = new UpdateRoundReopenCommandService(
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
            var fakeLogger = NullLogger<UpdateRoundReopenCommandService>.Instance;

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var currencyCode = "CNY";

            var command = new UpdateRoundReopenCommand(
                RoundId: roundId,
                Status: GameRoundStatus.Closed,
                ExternalClosedOn: DateTimeOffset.UtcNow,
                BetAmount: 1m,
                ValidBetAmount: 1m,
                SettleAmount: 2m,
                Commission: 0,
                Turnover: 1m,
                PlayerWinLoss: 1m,
                ClientStatus: GameRoundClientStatus.Win,
                AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult<bool>(true));

            _ = fakeGameRound
                .GetByCurrencyInfoAsync(command.RoundId)
                .Returns(ValueTask.FromResult<(Guid GameProviderId, string CurrencyCode)>(new(gameProviderId, currencyCode)));

            var gameRoundConvertRequest = new GameRoundConvertRequest(
                GameProviderId: gameProviderId,
                CurrencyCode: currencyCode,
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: command.SettleAmount,
                Commission: command.Commission,
                Turnover: command.Turnover,
                PlayerWinLoss: command.PlayerWinLoss);
            var gameRoundConvertResult = new GameRoundConvertResult(
                BetAmount: 1000m,
                ValidBetAmount: 1000m,
                SettleAmount: 2000m,
                Commission: 0m,
                Turnover: 1000m,
                PlayerWinLoss: 1000m);
            _ = fakeGameRoundConvert.ConvertPlatformRoundsAsync(
                request: gameRoundConvertRequest).Returns(gameRoundConvertResult);

            var updateRoundReopen = new UpdateRoundReopen(
                RoundId: command.RoundId,
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
            _ = fakeGameRound.UpdateRoundReopenAsync(updateRoundReopen);

            var service = new UpdateRoundReopenCommandService(
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
            var fakeLogger = NullLogger<UpdateRoundReopenCommandService>.Instance;

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var currencyCode = "CNY";

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

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult<bool>(true));

            _ = fakeGameRound
                .GetByCurrencyInfoAsync(command.RoundId)
                .Returns(ValueTask.FromResult<(Guid GameProviderId, string CurrencyCode)>(new(gameProviderId, currencyCode)));

            var gameRoundConvertRequest = new GameRoundConvertRequest(
                GameProviderId: gameProviderId,
                CurrencyCode: currencyCode,
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: command.SettleAmount,
                Commission: command.Commission,
                Turnover: command.Turnover,
                PlayerWinLoss: command.PlayerWinLoss);
            var gameRoundConvertResult = new GameRoundConvertResult(
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: command.SettleAmount,
                Commission: command.Commission,
                Turnover: command.Turnover,
                PlayerWinLoss: command.PlayerWinLoss);
            _ = fakeGameRoundConvert.ConvertPlatformRoundsAsync(
                request: gameRoundConvertRequest).Returns(gameRoundConvertResult);

            var updateRoundReopen = new UpdateRoundReopen(
                RoundId: command.RoundId,
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
            _ = fakeGameRound.UpdateRoundReopenAsync(updateRoundReopen);

            var service = new UpdateRoundReopenCommandService(
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
