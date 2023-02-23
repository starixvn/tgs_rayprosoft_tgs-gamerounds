using System;
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
public class AddRoundCommandServiceUnitTest
{
    [TestMethod]
    public async Task Add_Has_Been_Locked()
    {
        var fakeLogger = NullLogger<AddRoundCommandService>.Instance;
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var fakeGameRoundLocked = Substitute.For<IGameRoundLocked>();

        var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
        var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
        var gameId = 500001;
        var playerCode = "4ues8xikza7o";
        var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

        var command = new AddRoundCommand(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            PlayerCode: playerCode,
            GameId: gameId,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Open,
            GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
            WalletId: walletId,
            BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
            ExternalCreatedOn: DateTimeOffset.UtcNow,
            ExternalClosedOn: null,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: null,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: null,
            ClientStatus: GameRoundClientStatus.Open,
            AdditionalData: "{}");

        var expected = Guid.NewGuid();

        _ = fakeGameRoundLocked.LockingAsync(
            Arg.Is(command.GameProviderId),
            Arg.Is(command.ExternalRoundId),
            Arg.Is(command.GameId),
            Arg.Is(command.WalletId)).Returns(false);

        var service = new AddRoundCommandService(
            fakeLogger,
            fakeGameRound,
            fakeGameRoundLocked);

        _ = await Assert.ThrowsExceptionAsync<GameRoundCreationFailedException>(() => service.ExecuteAsync(command).AsTask()).ConfigureAwait(false);

        _ = fakeGameRoundLocked
            .Received(0)
            .UnlockAsync(
                Arg.Is(command.GameProviderId),
                Arg.Is(command.ExternalRoundId),
                Arg.Is(command.GameId),
                Arg.Is(command.WalletId));
    }

    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<AddRoundCommandService>.Instance;
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var fakeGameRoundLocked = Substitute.For<IGameRoundLocked>();

        var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
        var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
        var gameId = 500001;
        var playerCode = "4ues8xikza7o";
        var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

        var command = new AddRoundCommand(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            PlayerCode: playerCode,
            GameId: gameId,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Open,
            GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
            WalletId: walletId,
            BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
            ExternalCreatedOn: DateTimeOffset.UtcNow,
            ExternalClosedOn: null,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: null,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: null,
            ClientStatus: GameRoundClientStatus.Open,
            AdditionalData: "{}");

        var expected = Guid.NewGuid();

        _ = fakeGameRoundLocked.LockingAsync(
            Arg.Is(command.GameProviderId),
            Arg.Is(command.ExternalRoundId),
            Arg.Is(command.GameId),
            Arg.Is(command.WalletId)).Returns(true);

        var addRound = new AddRound(
            ExternalRoundId: command.ExternalRoundId,
            GameProviderId: command.GameProviderId,
            PlayerCode: command.PlayerCode,
            GameId: command.GameId,
            CurrencyCode: command.CurrencyCode,
            Status: command.Status,
            GameProviderConfigurationId: command.GameProviderConfigurationId,
            WalletId: command.WalletId,
            BrandId: command.BrandId,
            ExternalCreatedOn: command.ExternalCreatedOn,
            ExternalClosedOn: command.ExternalClosedOn,
            BetAmount: command.BetAmount,
            ValidBetAmount: command.ValidBetAmount,
            SettleAmount: command.SettleAmount,
            Commission: command.Commission,
            Turnover: command.Turnover,
            PlayerWinLoss: command.PlayerWinLoss,
            ClientStatus: command.ClientStatus,
            AdditionalData: command.AdditionalData);
        _ = fakeGameRound.AddRoundAsync(addRound).Returns(ValueTask.FromResult<Guid?>(expected));

        var service = new AddRoundCommandService(
            fakeLogger,
            fakeGameRound,
            fakeGameRoundLocked);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameRoundLocked
            .Received(1)
            .UnlockAsync(
                Arg.Is(command.GameProviderId),
                Arg.Is(command.ExternalRoundId),
                Arg.Is(command.GameId),
                Arg.Is(command.WalletId));
    }
}
