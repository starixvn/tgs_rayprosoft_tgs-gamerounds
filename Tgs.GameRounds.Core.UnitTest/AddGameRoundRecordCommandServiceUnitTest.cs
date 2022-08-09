using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.UnitTest
{
    [TestClass]
    public class AddGameRoundRecordCommandServiceUnitTest
    {
        [TestMethod]
        public async Task Database_Could_Not_Find_The_Game_Round_Data()
        {
            var fakeLogger = NullLogger<AddGameRoundRecordCommandService>.Instance;
            var fakeGame = Substitute.For<IGame>();
            var fakeGameProvider = Substitute.For<IGameProvider>();
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecord>();

            var roundId = Guid.NewGuid();

            var command = new AddGameRoundRecordCommand(
                RoundId: roundId,
                SendOn: DateTimeOffset.UtcNow.AddSeconds(-1),
                ReceiveOn: DateTimeOffset.UtcNow);

            _ = fakeGameRound.GetAsync(
                Arg.Is(roundId)).Returns(ValueTask.FromResult<GameRoundInfo?>(null));

            var service = new AddGameRoundRecordCommandService(
                fakeLogger,
                fakeGame,
                fakeGameProvider,
                fakeGameRound,
                fakeGameRoundRecord);

            await service.ExecuteAsync(command).ConfigureAwait(false);

            _ = fakeGameProvider
                .Received(0)
                .GetGameProviderCodeAsync(
                Arg.Any<Guid>());

            _ = fakeGame
                .Received(0)
                .GetExternalGameIdAsync(
                Arg.Any<int>());

            _ = fakeGameRoundRecord
                .Received(0)
                .AddAsync(Arg.Any<AddGameRoundRecord>());
        }

        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeLogger = NullLogger<AddGameRoundRecordCommandService>.Instance;
            var fakeGame = Substitute.For<IGame>();
            var fakeGameProvider = Substitute.For<IGameProvider>();
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecord>();

            var roundId = Guid.NewGuid();

            var command = new AddGameRoundRecordCommand(
                RoundId: roundId,
                SendOn: DateTimeOffset.UtcNow.AddSeconds(-1),
                ReceiveOn: DateTimeOffset.UtcNow);

            var gameRoundInfo = new GameRoundInfo(
                RoundId: roundId,
                ExternalRoundId: Guid.NewGuid().ToString("N"),
                GameProviderId: new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
                PlayerCode: "4ues8xikza7o",
                GameId: 500001,
                CurrencyCode: "CNY",
                Status: GameRoundStatus.Closed,
                GameProviderConfigurationId: Guid.NewGuid(),
                WalletId: Guid.NewGuid(),
                CreatedOn: DateTime.Now,
                ClosedOn: DateTime.Now,
                UpdatedOn: DateTime.Now,
                BrandId: Guid.NewGuid(),
                ExternalCreatedOn: DateTime.Now,
                ExternalClosedOn: DateTime.Now,
                BetAmount: 100,
                ValidBetAmount: 100,
                SettleAmount: 200,
                Commission: 0,
                Turnover: 100,
                CompanyWinLoss: -100,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Win,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                EffectiveBettingPercentage: null,
                AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId })
                );

            _ = fakeGameRound.GetAsync(
                Arg.Is(roundId)).Returns(gameRoundInfo);

            var gameProviderCode = "KY";
            _ = fakeGameProvider.GetGameProviderCodeAsync(
                Arg.Is(gameRoundInfo.GameProviderId)).Returns(gameProviderCode);

            var externalGameId = "Bacc";
            _ = fakeGame.GetExternalGameIdAsync(
                Arg.Is(gameRoundInfo.GameId)).Returns(externalGameId);

            var service = new AddGameRoundRecordCommandService(
                fakeLogger,
                fakeGame,
                fakeGameProvider,
                fakeGameRound,
                fakeGameRoundRecord);

            await service.ExecuteAsync(command).ConfigureAwait(false);

            var gameRoundRecordAdd = new AddGameRoundRecord(
                RoundId: gameRoundInfo.RoundId,
                ExternalRoundId: gameRoundInfo.ExternalRoundId,
                ExternalGameId: externalGameId,
                GameProviderId: gameRoundInfo.GameProviderId,
                GameProviderCode: gameProviderCode,
                GameProviderConfigurationId: gameRoundInfo.GameProviderConfigurationId,
                GameId: gameRoundInfo.GameId,
                BrandId: gameRoundInfo.BrandId,
                PlayerCode: gameRoundInfo.PlayerCode,
                WalletId: gameRoundInfo.WalletId,
                CurrencyCode: gameRoundInfo.CurrencyCode,
                Status: gameRoundInfo.Status,
                ClientStatus: gameRoundInfo.ClientStatus,
                CreatedOn: gameRoundInfo.CreatedOn,
                ClosedOn: gameRoundInfo.ClosedOn,
                UpdatedOn: gameRoundInfo.UpdatedOn,
                ExternalCreatedOn: gameRoundInfo.ExternalCreatedOn,
                ExternalClosedOn: gameRoundInfo.ExternalClosedOn,
                Bet: gameRoundInfo.BetAmount.GetValueOrDefault(0),
                ValidBet: gameRoundInfo.ValidBetAmount.GetValueOrDefault(0),
                Settle: gameRoundInfo.SettleAmount.GetValueOrDefault(0),
                Commission: gameRoundInfo.Commission.GetValueOrDefault(0),
                Turnover: gameRoundInfo.Turnover.GetValueOrDefault(0),
                CompanyWinLoss: gameRoundInfo.CompanyWinLoss.GetValueOrDefault(0),
                TieBetAdjustment: gameRoundInfo.TieBetAdjustment.GetValueOrDefault(0),
                ReopenTurnoverAdjustment: gameRoundInfo.ReopenTurnoverAdjustment.GetValueOrDefault(0),
                ReopenedOn: gameRoundInfo.ReopenedOn,
                SendOn: command.SendOn,
                ReceiveOn: command.ReceiveOn);

            _ = fakeGameRoundRecord
                .Received(1)
                .AddAsync(Arg.Is(gameRoundRecordAdd));
        }
    }
}
