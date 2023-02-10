using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.HandleServices;

namespace Tgs.GameRounds.Core.UnitTest;

[TestClass]
public class GameRoundCheckingHandleServiceUnitTest
{
    [TestMethod]
    public async Task Add_Round()
    {
        var fakeLogger = NullLogger<GameRoundCheckingHandleService>.Instance;
        var fakeRepository = Substitute.For<IGameRoundRepository>();

        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderId = Guid.NewGuid();
        var playerCode = "test";
        var gameId = 1;
        var walletId = Guid.NewGuid();

        var command = new GameRoundCheckingQuery(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            GameId: gameId,
            PlayerCode: playerCode,
            WalletId: walletId,
            Status: GameRoundStatus.Open,
            ClientStatus: GameRoundClientStatus.Open,
            ExternalCreatedOn: DateTimeOffset.UtcNow,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 0,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: 0);

        _ = fakeRepository
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(command.GameProviderId),
                externalRoundId: Arg.Is(command.ExternalRoundId),
                gameId: Arg.Is(command.GameId),
                walletId: Arg.Is(command.WalletId),
                externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(null));

        var service = new GameRoundCheckingHandleService(
            fakeLogger,
            fakeRepository);

        var actual = await service.GetAsync(command).ConfigureAwait(false);

        Assert.AreEqual(GameRoundCheckingStatus.Change, actual);
    }

    [TestMethod]
    public async Task Update_Round_Closed()
    {
        var fakeLogger = NullLogger<GameRoundCheckingHandleService>.Instance;
        var fakeRepository = Substitute.For<IGameRoundRepository>();

        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderId = Guid.NewGuid();
        var playerCode = "test";
        var gameId = 1;
        var walletId = Guid.NewGuid();

        var command = new GameRoundCheckingQuery(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            GameId: gameId,
            PlayerCode: playerCode,
            WalletId: walletId,
            Status: GameRoundStatus.Closed,
            ClientStatus: GameRoundClientStatus.Win,
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: 10m);

        var id = Guid.NewGuid();
        _ = fakeRepository
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(command.GameProviderId),
                externalRoundId: Arg.Is(command.ExternalRoundId),
                gameId: Arg.Is(command.GameId),
                walletId: Arg.Is(command.WalletId),
                externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(id));

        var info = new GameRoundInfo(
            RoundId: id,
            ExternalRoundId: command.ExternalRoundId,
            GameProviderId: command.GameProviderId,
            PlayerCode: command.PlayerCode,
            GameId: command.GameId,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Open,
            GameProviderConfigurationId: Guid.NewGuid(),
            WalletId: command.WalletId,
            CreatedOn: DateTimeOffset.UtcNow,
            ClosedOn: DateTimeOffset.UtcNow,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: Guid.NewGuid(),
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            ExternalClosedOn: null,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 0m,
            Commission: 0m,
            Turnover: 0m,
            CompanyWinLoss: 0m,
            TieBetAdjustment: 0m,
            ClientStatus: GameRoundClientStatus.Open,
            ReopenTurnoverAdjustment: 0,
            ReopenedOn: null,
            EffectiveBettingPercentage: 0);
        _ = fakeRepository
            .GetAsync(
                id: Arg.Is(id),
                externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<GameRoundInfo?>(info));

        var service = new GameRoundCheckingHandleService(
            fakeLogger,
            fakeRepository);

        var actual = await service.GetAsync(command).ConfigureAwait(false);

        Assert.AreEqual(GameRoundCheckingStatus.Change, actual);
    }

    [TestMethod]
    public async Task Update_Round_Closed_Get_DataBase_GameRoundStatus_Is_Closed()
    {
        var fakeLogger = NullLogger<GameRoundCheckingHandleService>.Instance;
        var fakeRepository = Substitute.For<IGameRoundRepository>();

        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderId = Guid.NewGuid();
        var playerCode = "test";
        var gameId = 1;
        var walletId = Guid.NewGuid();

        var command = new GameRoundCheckingQuery(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            GameId: gameId,
            PlayerCode: playerCode,
            WalletId: walletId,
            Status: GameRoundStatus.Closed,
            ClientStatus: GameRoundClientStatus.Win,
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: 10m);

        var id = Guid.NewGuid();
        _ = fakeRepository
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(command.GameProviderId),
                externalRoundId: Arg.Is(command.ExternalRoundId),
                gameId: Arg.Is(command.GameId),
                walletId: Arg.Is(command.WalletId),
                externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(id));

        var info = new GameRoundInfo(
            RoundId: id,
            ExternalRoundId: command.ExternalRoundId,
            GameProviderId: command.GameProviderId,
            PlayerCode: command.PlayerCode,
            GameId: command.GameId,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Closed,
            GameProviderConfigurationId: Guid.NewGuid(),
            WalletId: command.WalletId,
            CreatedOn: DateTimeOffset.UtcNow,
            ClosedOn: DateTimeOffset.UtcNow,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: Guid.NewGuid(),
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            ExternalClosedOn: null,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0m,
            Turnover: 10m,
            CompanyWinLoss: -10m,
            TieBetAdjustment: 0,
            ClientStatus: GameRoundClientStatus.Win,
            ReopenTurnoverAdjustment: 0,
            ReopenedOn: null,
            EffectiveBettingPercentage: 0);
        _ = fakeRepository
            .GetAsync(
                id: Arg.Is(id),
                externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<GameRoundInfo?>(info));

        var service = new GameRoundCheckingHandleService(
            fakeLogger,
            fakeRepository);

        var actual = await service.GetAsync(command).ConfigureAwait(false);

        Assert.AreEqual(GameRoundCheckingStatus.Unchanged, actual);
    }

    [TestMethod]
    public async Task Update_Round_Open()
    {
        var fakeLogger = NullLogger<GameRoundCheckingHandleService>.Instance;
        var fakeRepository = Substitute.For<IGameRoundRepository>();

        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderId = Guid.NewGuid();
        var playerCode = "test";
        var gameId = 1;
        var walletId = Guid.NewGuid();

        var command = new GameRoundCheckingQuery(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            GameId: gameId,
            PlayerCode: playerCode,
            WalletId: walletId,
            Status: GameRoundStatus.Open,
            ClientStatus: GameRoundClientStatus.Open,
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            BetAmount: 20m,
            ValidBetAmount: 20m,
            SettleAmount: 0m,
            Commission: 0,
            Turnover: 0m,
            PlayerWinLoss: 0m);

        var id = Guid.NewGuid();
        _ = fakeRepository
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(command.GameProviderId),
                externalRoundId: Arg.Is(command.ExternalRoundId),
                gameId: Arg.Is(command.GameId),
                walletId: Arg.Is(command.WalletId),
                externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(id));

        var info = new GameRoundInfo(
            RoundId: id,
            ExternalRoundId: command.ExternalRoundId,
            GameProviderId: command.GameProviderId,
            PlayerCode: command.PlayerCode,
            GameId: command.GameId,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Open,
            GameProviderConfigurationId: Guid.NewGuid(),
            WalletId: command.WalletId,
            CreatedOn: DateTimeOffset.UtcNow,
            ClosedOn: DateTimeOffset.UtcNow,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: Guid.NewGuid(),
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            ExternalClosedOn: null,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 0m,
            Commission: 0m,
            Turnover: 0m,
            CompanyWinLoss: 0m,
            TieBetAdjustment: 0,
            ClientStatus: GameRoundClientStatus.Open,
            ReopenTurnoverAdjustment: 0,
            ReopenedOn: null,
            EffectiveBettingPercentage: 0);
        _ = fakeRepository
            .GetAsync(
                id: Arg.Is(id),
                externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<GameRoundInfo?>(info));

        var service = new GameRoundCheckingHandleService(
            fakeLogger,
            fakeRepository);

        var actual = await service.GetAsync(command).ConfigureAwait(false);

        Assert.AreEqual(GameRoundCheckingStatus.Change, actual);
    }

    [TestMethod]
    public async Task Update_Round_Reopen()
    {
        var fakeLogger = NullLogger<GameRoundCheckingHandleService>.Instance;
        var fakeRepository = Substitute.For<IGameRoundRepository>();

        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderId = Guid.NewGuid();
        var playerCode = "test";
        var gameId = 1;
        var walletId = Guid.NewGuid();

        var command = new GameRoundCheckingQuery(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            GameId: gameId,
            PlayerCode: playerCode,
            WalletId: walletId,
            Status: GameRoundStatus.Closed,
            ClientStatus: GameRoundClientStatus.Win,
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: 10m);

        var id = Guid.NewGuid();
        _ = fakeRepository
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(command.GameProviderId),
                externalRoundId: Arg.Is(command.ExternalRoundId),
                gameId: Arg.Is(command.GameId),
                walletId: Arg.Is(command.WalletId),
                externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(id));

        var info = new GameRoundInfo(
            RoundId: id,
            ExternalRoundId: command.ExternalRoundId,
            GameProviderId: command.GameProviderId,
            PlayerCode: command.PlayerCode,
            GameId: command.GameId,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Closed,
            GameProviderConfigurationId: Guid.NewGuid(),
            WalletId: command.WalletId,
            CreatedOn: DateTimeOffset.UtcNow,
            ClosedOn: DateTimeOffset.UtcNow,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: Guid.NewGuid(),
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            ExternalClosedOn: null,
            BetAmount: 20m,
            ValidBetAmount: 20m,
            SettleAmount: 0m,
            Commission: 0m,
            Turnover: 0m,
            CompanyWinLoss: -20m,
            TieBetAdjustment: 0,
            ClientStatus: GameRoundClientStatus.Loss,
            ReopenTurnoverAdjustment: 0,
            ReopenedOn: null,
            EffectiveBettingPercentage: 0);
        _ = fakeRepository
            .GetAsync(
                id: Arg.Is(id),
                externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<GameRoundInfo?>(info));

        var service = new GameRoundCheckingHandleService(
            fakeLogger,
            fakeRepository);

        var actual = await service.GetAsync(command).ConfigureAwait(false);

        Assert.AreEqual(GameRoundCheckingStatus.Change, actual);
    }

    [TestMethod]
    public async Task Update_Round_Reopen_Is_GameRoundStatus_Open()
    {
        var fakeLogger = NullLogger<GameRoundCheckingHandleService>.Instance;
        var fakeRepository = Substitute.For<IGameRoundRepository>();

        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderId = Guid.NewGuid();
        var playerCode = "test";
        var gameId = 1;
        var walletId = Guid.NewGuid();

        var command = new GameRoundCheckingQuery(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            GameId: gameId,
            PlayerCode: playerCode,
            WalletId: walletId,
            Status: GameRoundStatus.Open,
            ClientStatus: GameRoundClientStatus.Open,
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 0m,
            Commission: 0,
            Turnover: 0m,
            PlayerWinLoss: 0m);

        var id = Guid.NewGuid();
        _ = fakeRepository
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(command.GameProviderId),
                externalRoundId: Arg.Is(command.ExternalRoundId),
                gameId: Arg.Is(command.GameId),
                walletId: Arg.Is(command.WalletId),
                externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(id));

        var info = new GameRoundInfo(
            RoundId: id,
            ExternalRoundId: command.ExternalRoundId,
            GameProviderId: command.GameProviderId,
            PlayerCode: command.PlayerCode,
            GameId: command.GameId,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Closed,
            GameProviderConfigurationId: Guid.NewGuid(),
            WalletId: command.WalletId,
            CreatedOn: DateTimeOffset.UtcNow,
            ClosedOn: DateTimeOffset.UtcNow,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: Guid.NewGuid(),
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            ExternalClosedOn: null,
            BetAmount: 20m,
            ValidBetAmount: 20m,
            SettleAmount: 0m,
            Commission: 0m,
            Turnover: 0m,
            CompanyWinLoss: -20m,
            TieBetAdjustment: 0,
            ClientStatus: GameRoundClientStatus.Loss,
            ReopenTurnoverAdjustment: 0,
            ReopenedOn: null,
            EffectiveBettingPercentage: 0);
        _ = fakeRepository
            .GetAsync(
                id: Arg.Is(id),
                externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<GameRoundInfo?>(info));

        var service = new GameRoundCheckingHandleService(
            fakeLogger,
            fakeRepository);

        var actual = await service.GetAsync(command).ConfigureAwait(false);

        Assert.AreEqual(GameRoundCheckingStatus.Unchanged, actual);
    }
}
