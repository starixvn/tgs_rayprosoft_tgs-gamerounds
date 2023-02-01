using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.UnitTest;

[TestClass]
public class AddGameRoundRecordCommandServiceUnitTest
{
    [TestMethod]
    public async Task Database_Could_Not_Find_The_Game_Round_Data()
    {
        var fakeLogger = NullLogger<AddGameRoundRecordCommandService>.Instance;
        var fakeGame = Substitute.For<IGameRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
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
            .GetCode(
            Arg.Any<Guid>());

        _ = fakeGame
            .Received(0)
            .GetExternalIdAsync(
            Arg.Any<int>());

        _ = fakeGameRoundRecord
            .Received(0)
            .AddAsync(Arg.Any<AddGameRoundRecord>());
    }

    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<AddGameRoundRecordCommandService>.Instance;
        var fakeGame = Substitute.For<IGameRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
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
            TieBetAdjustment: 0,
            ClientStatus: GameRoundClientStatus.Win,
            ReopenTurnoverAdjustment: null,
            ReopenedOn: null,
            EffectiveBettingPercentage: null);

        _ = fakeGameRound.GetAsync(
            Arg.Is(roundId)).Returns(gameRoundInfo);

        var gameProviderCode = "KY";
        _ = fakeGameProvider.GetCode(
            Arg.Is(gameRoundInfo.GameProviderId)).Returns(gameProviderCode);

        var externalGameId = "Bacc";
        _ = fakeGame.GetExternalIdAsync(
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
            Bet: gameRoundInfo.BetAmount,
            ValidBet: gameRoundInfo.ValidBetAmount,
            Settle: gameRoundInfo.SettleAmount,
            Commission: gameRoundInfo.Commission,
            Turnover: gameRoundInfo.Turnover,
            CompanyWinLoss: gameRoundInfo.CompanyWinLoss,
            TieBetAdjustment: gameRoundInfo.TieBetAdjustment,
            ReopenTurnoverAdjustment: gameRoundInfo.ReopenTurnoverAdjustment.GetValueOrDefault(0),
            ReopenedOn: gameRoundInfo.ReopenedOn,
            SendOn: command.SendOn,
            ReceiveOn: command.ReceiveOn);

        _ = fakeGameRoundRecord
            .Received(1)
            .AddAsync(Arg.Is(gameRoundRecordAdd));
    }
}
