using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;

namespace Tgs.GameRounds.Core.UnitTest
{
    [TestClass]
    public class UpdateRoundCommandServiceUnitTest
    {
        [TestMethod]
        public async Task ExternalRoundId_Not_Exist()
        {
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeLogger = NullLogger<UpdateRoundCommandService>.Instance;

            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var roundId = Guid.NewGuid();

            var command = new UpdateRoundCommand(
                RoundId: roundId,
                Status: GameRoundStatus.Closed,
                ClosedOn: DateTimeOffset.UtcNow,
                ExternalClosedOn: DateTimeOffset.UtcNow,
                BetAmount: 20m,
                ValidBetAmount: 20m,
                SettleAmount: 40m,
                Commission: 0m,
                Turnover: 20m,
                WinLoss: -20m,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Win,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                AdditionalData: null
                );

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult(false));

            var service = new UpdateRoundCommandService(
                fakeGameRoundRecord,
                fakeGameRound,
                fakeLogger);

            _ = await Assert.ThrowsExceptionAsync<GameRoundDoesNotExistException>(() => service.ExecuteAsync(command).AsTask()).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(0).AddAsync(Arg.Any<GameRoundInfo>());
        }

        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeLogger = NullLogger<UpdateRoundCommandService>.Instance;

            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var command = new UpdateRoundCommand(
                RoundId: roundId,
                Status: GameRoundStatus.Closed,
                ClosedOn: DateTimeOffset.UtcNow,
                ExternalClosedOn: DateTimeOffset.UtcNow,
                BetAmount: 20m,
                ValidBetAmount: 20m,
                SettleAmount: 40m,
                Commission: 0m,
                Turnover: 20m,
                WinLoss: -20m,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Win,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                AdditionalData: null
                );

            var gameRound = new GameRoundInfo(
                RoundId: command.RoundId,
                ExternalRoundId: externalRoundId,
                GameProviderId: gameProviderId,
                PlayerCode: playerCode,
                GameId: gameId,
                CurrencyCode: "CNY",
                Status: GameRoundStatus.Open,
                GameProviderConfigurationId: Guid.NewGuid(),
                WalletId: Guid.NewGuid(),
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: command.ClosedOn,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: Guid.NewGuid(),
                ExternalCreatedOn: DateTimeOffset.UtcNow,
                ExternalClosedOn: command.ExternalClosedOn,
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: command.SettleAmount,
                Commission: command.Commission,
                Turnover: command.Turnover,
                WinLoss: command.WinLoss,
                TieBetAdjustment: command.TieBetAdjustment,
                ClientStatus: GameRoundClientStatus.Open,
                ReopenTurnoverAdjustment: command.ReopenTurnoverAdjustment,
                ReopenedOn: command.ReopenedOn,
                EffectiveBettingPercentage: null,
                AdditionalData: command.AdditionalData ?? string.Empty);

            var expected = true;

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult<bool>(expected));

            _ = fakeGameRound.UpdateRoundAsync(new(
                command.RoundId,
                command.Status,
                command.ClosedOn,
                command.ExternalClosedOn,
                command.BetAmount,
                command.ValidBetAmount,
                command.SettleAmount,
                command.Commission,
                command.Turnover,
                command.WinLoss,
                command.TieBetAdjustment,
                command.ClientStatus,
                command.ReopenTurnoverAdjustment,
                command.ReopenedOn,
                command.AdditionalData));

            _ = fakeGameRound.GetAsync(command.RoundId).Returns(ValueTask.FromResult<GameRoundInfo?>(gameRound));

            var service = new UpdateRoundCommandService(
                fakeGameRoundRecord,
                fakeGameRound,
                fakeLogger);

            await service.ExecuteAsync(command).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Is(gameRound));
        }
    }
}
