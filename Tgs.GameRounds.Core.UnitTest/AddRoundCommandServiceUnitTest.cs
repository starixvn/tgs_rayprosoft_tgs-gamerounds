using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;

namespace Tgs.GameRounds.Core.UnitTest
{
    [TestClass]
    public class AddRoundCommandServiceUnitTest
    {
        [TestMethod]
        public async Task Add_Has_Been_Locked()
        {
            var fakeGameRoundCache = Substitute.For<IGameRoundService>();
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRoundRepository>();

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
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: null,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn: DateTimeOffset.UtcNow,
                ExternalClosedOn: null,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                SettleAmount: null,
                Commission: 0,
                Turnover: 10m,
                WinLoss: null,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Open,
                AdditionalData: "{}");

            var expected = Guid.NewGuid();

            var gameRound = new GameRoundInfo(
                RoundId: expected,
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
                BetAmount: null,
                ValidBetAmount: null,
                SettleAmount: null,
                Commission: null,
                Turnover: null,
                WinLoss: null,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Open,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                EffectiveBettingPercentage: null,
                AdditionalData: string.Empty
                );

            _ = fakeGameRoundCache.LockAsync(
                command.GameProviderId,
                command.ExternalRoundId,
                command.GameId,
                command.WalletId).Returns(false);

            var service = new AddRoundCommandService(
                fakeGameRoundCache,
                fakeGameRoundRecord,
                fakeGameRound);

            await service.ExecuteAsync(command).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(0).AddAsync(Arg.Is(gameRound));

            _ = fakeGameRoundCache
                .Received(0)
                .UnLockAsync(
                    command.GameProviderId,
                    command.ExternalRoundId,
                    command.GameId,
                    command.WalletId);
        }

        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameRoundCache = Substitute.For<IGameRoundService>();
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRoundRepository>();

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
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: null,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn: DateTimeOffset.UtcNow,
                ExternalClosedOn: null,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                SettleAmount: null,
                Commission: 0,
                Turnover: 10m,
                WinLoss: null,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Open,
                AdditionalData: "{}");

            var expected = Guid.NewGuid();

            var gameRound = new GameRoundInfo(
                RoundId: expected,
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
                BetAmount: null,
                ValidBetAmount: null,
                SettleAmount: null,
                Commission: null,
                Turnover: null,
                WinLoss: null,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Open,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                EffectiveBettingPercentage: null,
                AdditionalData: string.Empty
                );

            _ = fakeGameRoundCache.LockAsync(
                command.GameProviderId,
                command.ExternalRoundId,
                command.GameId,
                command.WalletId).Returns(true);

            _ = fakeGameRound.AddRoundAsync(
                new(command.ExternalRoundId,
                    command.GameProviderId,
                    command.PlayerCode,
                    command.GameId,
                    command.CurrencyCode,
                    command.Status,
                    command.GameProviderConfigurationId,
                    command.WalletId,
                    command.CreatedOn,
                    command.ClosedOn,
                    command.UpdatedOn,
                    command.BrandId,
                    command.ExternalCreatedOn,
                    command.ExternalClosedOn,
                    command.BetAmount,
                    command.ValidBetAmount,
                    command.SettleAmount,
                    command.Commission,
                    command.Turnover,
                    command.WinLoss,
                    command.TieBetAdjustment,
                    command.ClientStatus,
                    command.AdditionalData)).Returns(ValueTask.FromResult(expected));
            _ = fakeGameRound.GetAsync(expected).Returns(ValueTask.FromResult(gameRound));

            var service = new AddRoundCommandService(
                fakeGameRoundCache,
                fakeGameRoundRecord,
                fakeGameRound);

            await service.ExecuteAsync(command).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Is(gameRound));

            _ = fakeGameRoundCache
                .Received(1)
                .UnLockAsync(
                    command.GameProviderId,
                    command.ExternalRoundId,
                    command.GameId,
                    command.WalletId);
        }
    }
}
