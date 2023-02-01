using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.HandleServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.UnitTest;

[TestClass]
public class GameRoundsWorkCommandServiceUnitTest
{
    [TestMethod]
    public async Task Add_Round()
    {
        var fakeLogger = NullLogger<GameRoundWorkHandleService>.Instance;
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var fakeAddRoundService = Substitute.For<ICommandService<AddRoundCommand>>();
        var fakeUpdateRoundClosedService = Substitute.For<ICommandService<UpdateRoundClosedCommand>>();
        var fakeUpdateRoundOpenService = Substitute.For<ICommandService<UpdateRoundOpenCommand>>();
        var fakeUpdateRoundReopenService = Substitute.For<ICommandService<UpdateRoundReopenCommand>>();

        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderId = Guid.NewGuid();
        var playerCode = "test";
        var gameId = 1;
        var walletId = Guid.NewGuid();

        var command = new GameRoundWorkCommand(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            GameId: gameId,
            PlayerCode: playerCode,
            CurrencyCode: "CNY",
            WalletId: walletId,
            BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
            GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
            Status: GameRoundStatus.Open,
            ClientStatus: GameRoundClientStatus.Open,
            ExternalCreatedOn: DateTimeOffset.UtcNow,
            ExternalClosedOn: null,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 0,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: 0,
            AdditionalData: "{}");

        _ = fakeGameRound
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(command.GameProviderId),
                externalRoundId: Arg.Is(command.ExternalRoundId),
                gameId: Arg.Is(command.GameId),
                walletId: Arg.Is(command.WalletId))
            .Returns(ValueTask.FromResult<Guid?>(null));

        var service = new GameRoundWorkHandleService(
            fakeLogger,
            fakeGameRound,
            fakeAddRoundService,
            fakeUpdateRoundOpenService,
            fakeUpdateRoundClosedService,
            fakeUpdateRoundReopenService);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeAddRoundService
            .Received(1)
            .ExecuteAsync(Arg.Any<AddRoundCommand>());
        _ = fakeUpdateRoundOpenService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundOpenCommand>());
        _ = fakeUpdateRoundClosedService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundClosedCommand>());
        _ = fakeUpdateRoundReopenService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundReopenCommand>());
    }

    [TestMethod]
    public async Task Update_Round_Closed()
    {
        var fakeLogger = NullLogger<GameRoundWorkHandleService>.Instance;
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var fakeAddRoundService = Substitute.For<ICommandService<AddRoundCommand>>();
        var fakeUpdateRoundClosedService = Substitute.For<ICommandService<UpdateRoundClosedCommand>>();
        var fakeUpdateRoundOpenService = Substitute.For<ICommandService<UpdateRoundOpenCommand>>();
        var fakeUpdateRoundReopenService = Substitute.For<ICommandService<UpdateRoundReopenCommand>>();

        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderId = Guid.NewGuid();
        var playerCode = "test";
        var gameId = 1;
        var walletId = Guid.NewGuid();
        var roundId = Guid.NewGuid();

        var command = new GameRoundWorkCommand(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            GameId: gameId,
            PlayerCode: playerCode,
            CurrencyCode: "CNY",
            WalletId: walletId,
            BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
            GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
            Status: GameRoundStatus.Closed,
            ClientStatus: GameRoundClientStatus.Win,
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(0.5),
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: 10m,
            AdditionalData: "{}");

        _ = fakeGameRound
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(command.GameProviderId),
                externalRoundId: Arg.Is(command.ExternalRoundId),
                gameId: Arg.Is(command.GameId),
                walletId: Arg.Is(command.WalletId))
            .Returns(ValueTask.FromResult<Guid?>(roundId));

        var gameRound = new GameRoundInfo(
            RoundId: roundId,
            ExternalRoundId: command.ExternalRoundId,
            GameProviderId: command.GameProviderId,
            PlayerCode: command.PlayerCode,
            GameId: command.GameId,
            CurrencyCode: command.CurrencyCode,
            Status: GameRoundStatus.Open,
            GameProviderConfigurationId: command.GameProviderConfigurationId,
            WalletId: command.WalletId,
            CreatedOn: DateTimeOffset.UtcNow,
            ClosedOn: DateTimeOffset.UtcNow,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: command.BrandId,
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            ExternalClosedOn: null,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0m,
            Turnover: 0m,
            CompanyWinLoss: 0m,
            TieBetAdjustment: 0,
            ClientStatus: GameRoundClientStatus.Open,
            ReopenTurnoverAdjustment: 0,
            ReopenedOn: null,
            EffectiveBettingPercentage: 0);

        _ = fakeGameRound
            .GetAsync(
                roundId: Arg.Is(roundId))
            .Returns(gameRound);

        var service = new GameRoundWorkHandleService(
            fakeLogger,
            fakeGameRound,
            fakeAddRoundService,
            fakeUpdateRoundOpenService,
            fakeUpdateRoundClosedService,
            fakeUpdateRoundReopenService);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeAddRoundService
            .Received(0)
            .ExecuteAsync(Arg.Any<AddRoundCommand>());
        _ = fakeUpdateRoundOpenService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundOpenCommand>());
        _ = fakeUpdateRoundClosedService
            .Received(1)
            .ExecuteAsync(Arg.Any<UpdateRoundClosedCommand>());
        _ = fakeUpdateRoundReopenService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundReopenCommand>());
    }

    [TestMethod]
    public async Task Update_Round_Closed_Get_DataBase_GameRoundStatus_Is_Closed()
    {
        var fakeLogger = NullLogger<GameRoundWorkHandleService>.Instance;
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var fakeAddRoundService = Substitute.For<ICommandService<AddRoundCommand>>();
        var fakeUpdateRoundClosedService = Substitute.For<ICommandService<UpdateRoundClosedCommand>>();
        var fakeUpdateRoundOpenService = Substitute.For<ICommandService<UpdateRoundOpenCommand>>();
        var fakeUpdateRoundReopenService = Substitute.For<ICommandService<UpdateRoundReopenCommand>>();

        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderId = Guid.NewGuid();
        var playerCode = "test";
        var gameId = 1;
        var walletId = Guid.NewGuid();
        var roundId = Guid.NewGuid();

        var command = new GameRoundWorkCommand(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            GameId: gameId,
            PlayerCode: playerCode,
            CurrencyCode: "CNY",
            WalletId: walletId,
            BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
            GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
            Status: GameRoundStatus.Closed,
            ClientStatus: GameRoundClientStatus.Win,
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(0.5),
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: 10m,
            AdditionalData: "{}");

        _ = fakeGameRound
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(command.GameProviderId),
                externalRoundId: Arg.Is(command.ExternalRoundId),
                gameId: Arg.Is(command.GameId),
                walletId: Arg.Is(command.WalletId))
            .Returns(ValueTask.FromResult<Guid?>(roundId));

        var gameRound = new GameRoundInfo(
            RoundId: roundId,
            ExternalRoundId: command.ExternalRoundId,
            GameProviderId: command.GameProviderId,
            PlayerCode: command.PlayerCode,
            GameId: command.GameId,
            CurrencyCode: command.CurrencyCode,
            Status: GameRoundStatus.Closed,
            GameProviderConfigurationId: command.GameProviderConfigurationId,
            WalletId: command.WalletId,
            CreatedOn: DateTimeOffset.UtcNow,
            ClosedOn: DateTimeOffset.UtcNow,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: command.BrandId,
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(0.5),
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

        _ = fakeGameRound
            .GetAsync(
                roundId: Arg.Is(roundId))
            .Returns(gameRound);

        var service = new GameRoundWorkHandleService(
            fakeLogger,
            fakeGameRound,
            fakeAddRoundService,
            fakeUpdateRoundOpenService,
            fakeUpdateRoundClosedService,
            fakeUpdateRoundReopenService);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeAddRoundService
            .Received(0)
            .ExecuteAsync(Arg.Any<AddRoundCommand>());
        _ = fakeUpdateRoundOpenService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundOpenCommand>());
        _ = fakeUpdateRoundClosedService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundClosedCommand>());
        _ = fakeUpdateRoundReopenService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundReopenCommand>());
    }

    [TestMethod]
    public async Task Update_Round_Open()
    {
        var fakeLogger = NullLogger<GameRoundWorkHandleService>.Instance;
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var fakeAddRoundService = Substitute.For<ICommandService<AddRoundCommand>>();
        var fakeUpdateRoundClosedService = Substitute.For<ICommandService<UpdateRoundClosedCommand>>();
        var fakeUpdateRoundOpenService = Substitute.For<ICommandService<UpdateRoundOpenCommand>>();
        var fakeUpdateRoundReopenService = Substitute.For<ICommandService<UpdateRoundReopenCommand>>();

        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderId = Guid.NewGuid();
        var playerCode = "test";
        var gameId = 1;
        var walletId = Guid.NewGuid();
        var roundId = Guid.NewGuid();

        var command = new GameRoundWorkCommand(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            GameId: gameId,
            PlayerCode: playerCode,
            CurrencyCode: "CNY",
            WalletId: walletId,
            BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
            GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
            Status: GameRoundStatus.Open,
            ClientStatus: GameRoundClientStatus.Open,
            ExternalCreatedOn: DateTimeOffset.UtcNow,
            ExternalClosedOn: null,
            BetAmount: 20m,
            ValidBetAmount: 20m,
            SettleAmount: 0,
            Commission: 0,
            Turnover: 0,
            PlayerWinLoss: 0,
            AdditionalData: "{}");

        _ = fakeGameRound
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(command.GameProviderId),
                externalRoundId: Arg.Is(command.ExternalRoundId),
                gameId: Arg.Is(command.GameId),
                walletId: Arg.Is(command.WalletId))
            .Returns(ValueTask.FromResult<Guid?>(roundId));

        var gameRound = new GameRoundInfo(
            RoundId: roundId,
            ExternalRoundId: command.ExternalRoundId,
            GameProviderId: command.GameProviderId,
            PlayerCode: command.PlayerCode,
            GameId: command.GameId,
            CurrencyCode: command.CurrencyCode,
            Status: GameRoundStatus.Open,
            GameProviderConfigurationId: command.GameProviderConfigurationId,
            WalletId: command.WalletId,
            CreatedOn: DateTimeOffset.UtcNow,
            ClosedOn: null,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: command.BrandId,
            ExternalCreatedOn: command.ExternalCreatedOn,
            ExternalClosedOn: null,
            BetAmount: command.BetAmount,
            ValidBetAmount: 10m,
            SettleAmount: 10m,
            Commission: 0,
            Turnover: 0,
            CompanyWinLoss: 0,
            TieBetAdjustment: 0,
            ClientStatus: GameRoundClientStatus.Open,
            ReopenTurnoverAdjustment: 0,
            ReopenedOn: null,
            EffectiveBettingPercentage: 0);

        _ = fakeGameRound
            .GetAsync(
                roundId: Arg.Is(roundId))
            .Returns(gameRound);

        var service = new GameRoundWorkHandleService(
            fakeLogger,
            fakeGameRound,
            fakeAddRoundService,
            fakeUpdateRoundOpenService,
            fakeUpdateRoundClosedService,
            fakeUpdateRoundReopenService);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeAddRoundService
            .Received(0)
            .ExecuteAsync(Arg.Any<AddRoundCommand>());
        _ = fakeUpdateRoundOpenService
            .Received(1)
            .ExecuteAsync(Arg.Any<UpdateRoundOpenCommand>());
        _ = fakeUpdateRoundClosedService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundClosedCommand>());
        _ = fakeUpdateRoundReopenService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundReopenCommand>());
    }

    [TestMethod]
    public async Task Update_Round_Reopen()
    {
        var fakeLogger = NullLogger<GameRoundWorkHandleService>.Instance;
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var fakeAddRoundService = Substitute.For<ICommandService<AddRoundCommand>>();
        var fakeUpdateRoundClosedService = Substitute.For<ICommandService<UpdateRoundClosedCommand>>();
        var fakeUpdateRoundOpenService = Substitute.For<ICommandService<UpdateRoundOpenCommand>>();
        var fakeUpdateRoundReopenService = Substitute.For<ICommandService<UpdateRoundReopenCommand>>();

        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderId = Guid.NewGuid();
        var playerCode = "test";
        var gameId = 1;
        var walletId = Guid.NewGuid();
        var roundId = Guid.NewGuid();

        var command = new GameRoundWorkCommand(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            GameId: gameId,
            PlayerCode: playerCode,
            CurrencyCode: "CNY",
            WalletId: walletId,
            BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
            GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
            Status: GameRoundStatus.Closed,
            ClientStatus: GameRoundClientStatus.Loss,
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(0.5),
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 0m,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: -10m,
            AdditionalData: "{}");

        _ = fakeGameRound
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(command.GameProviderId),
                externalRoundId: Arg.Is(command.ExternalRoundId),
                gameId: Arg.Is(command.GameId),
                walletId: Arg.Is(command.WalletId))
            .Returns(ValueTask.FromResult<Guid?>(roundId));

        var gameRound = new GameRoundInfo(
            RoundId: roundId,
            ExternalRoundId: command.ExternalRoundId,
            GameProviderId: command.GameProviderId,
            PlayerCode: command.PlayerCode,
            GameId: command.GameId,
            CurrencyCode: command.CurrencyCode,
            Status: GameRoundStatus.Closed,
            GameProviderConfigurationId: command.GameProviderConfigurationId,
            WalletId: command.WalletId,
            CreatedOn: DateTimeOffset.UtcNow,
            ClosedOn: DateTimeOffset.UtcNow,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: command.BrandId,
            ExternalCreatedOn: command.ExternalCreatedOn,
            ExternalClosedOn: command.ExternalClosedOn,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0m,
            Turnover: 0m,
            CompanyWinLoss: -10m,
            TieBetAdjustment: 0,
            ClientStatus: GameRoundClientStatus.Win,
            ReopenTurnoverAdjustment: 0,
            ReopenedOn: null,
            EffectiveBettingPercentage: 0);

        _ = fakeGameRound
            .GetAsync(
                roundId: Arg.Is(roundId))
            .Returns(gameRound);

        var service = new GameRoundWorkHandleService(
            fakeLogger,
            fakeGameRound,
            fakeAddRoundService,
            fakeUpdateRoundOpenService,
            fakeUpdateRoundClosedService,
            fakeUpdateRoundReopenService);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeAddRoundService
            .Received(0)
            .ExecuteAsync(Arg.Any<AddRoundCommand>());
        _ = fakeUpdateRoundOpenService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundOpenCommand>());
        _ = fakeUpdateRoundClosedService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundClosedCommand>());
        _ = fakeUpdateRoundReopenService
            .Received(1)
            .ExecuteAsync(Arg.Any<UpdateRoundReopenCommand>());
    }

    [TestMethod]
    public async Task Update_Round_Reopen_Is_GameRoundStatus_Open()
    {
        var fakeLogger = NullLogger<GameRoundWorkHandleService>.Instance;
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var fakeAddRoundService = Substitute.For<ICommandService<AddRoundCommand>>();
        var fakeUpdateRoundClosedService = Substitute.For<ICommandService<UpdateRoundClosedCommand>>();
        var fakeUpdateRoundOpenService = Substitute.For<ICommandService<UpdateRoundOpenCommand>>();
        var fakeUpdateRoundReopenService = Substitute.For<ICommandService<UpdateRoundReopenCommand>>();

        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderId = Guid.NewGuid();
        var playerCode = "test";
        var gameId = 1;
        var walletId = Guid.NewGuid();
        var roundId = Guid.NewGuid();

        var command = new GameRoundWorkCommand(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            GameId: gameId,
            PlayerCode: playerCode,
            CurrencyCode: "CNY",
            WalletId: walletId,
            BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
            GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
            Status: GameRoundStatus.Open,
            ClientStatus: GameRoundClientStatus.Open,
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
            ExternalClosedOn: DateTimeOffset.UtcNow.AddMinutes(0.5),
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 0m,
            Commission: 0,
            Turnover: 0m,
            PlayerWinLoss: 0m,
            AdditionalData: "{}");

        _ = fakeGameRound
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(command.GameProviderId),
                externalRoundId: Arg.Is(command.ExternalRoundId),
                gameId: Arg.Is(command.GameId),
                walletId: Arg.Is(command.WalletId))
            .Returns(ValueTask.FromResult<Guid?>(roundId));

        var gameRound = new GameRoundInfo(
            RoundId: roundId,
            ExternalRoundId: command.ExternalRoundId,
            GameProviderId: command.GameProviderId,
            PlayerCode: command.PlayerCode,
            GameId: command.GameId,
            CurrencyCode: command.CurrencyCode,
            Status: GameRoundStatus.Closed,
            GameProviderConfigurationId: command.GameProviderConfigurationId,
            WalletId: command.WalletId,
            CreatedOn: DateTimeOffset.UtcNow,
            ClosedOn: DateTimeOffset.UtcNow,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: command.BrandId,
            ExternalCreatedOn: command.ExternalCreatedOn,
            ExternalClosedOn: command.ExternalClosedOn,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0m,
            Turnover: 0m,
            CompanyWinLoss: -10m,
            TieBetAdjustment: 0,
            ClientStatus: GameRoundClientStatus.Win,
            ReopenTurnoverAdjustment: 0,
            ReopenedOn: null,
            EffectiveBettingPercentage: 0);

        _ = fakeGameRound
            .GetAsync(
                roundId: Arg.Is(roundId))
            .Returns(gameRound);

        var service = new GameRoundWorkHandleService(
            fakeLogger,
            fakeGameRound,
            fakeAddRoundService,
            fakeUpdateRoundOpenService,
            fakeUpdateRoundClosedService,
            fakeUpdateRoundReopenService);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeAddRoundService
            .Received(0)
            .ExecuteAsync(Arg.Any<AddRoundCommand>());
        _ = fakeUpdateRoundOpenService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundOpenCommand>());
        _ = fakeUpdateRoundClosedService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundClosedCommand>());
        _ = fakeUpdateRoundReopenService
            .Received(0)
            .ExecuteAsync(Arg.Any<UpdateRoundReopenCommand>());
    }
}
