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

            var data = new AddRoundCommand(
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
                ExternalRoundId: data.ExternalRoundId,
                GameProviderId: data.GameProviderId,
                PlayerCode: data.PlayerCode,
                GameId: data.GameId,
                CurrencyCode: data.CurrencyCode,
                Status: GameRoundStatus.Open,
                GameProviderConfigurationId: data.GameProviderConfigurationId,
                WalletId: data.WalletId,
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: null,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: data.BrandId,
                ExternalCreatedOn: data.ExternalCreatedOn,
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

            var service = new AddRoundCommandService(
                fakeGameRoundCache,
                fakeGameRoundRecord,
                fakeGameRound);

            _ = fakeGameRound.AddRoundAsync(
                new(data.ExternalRoundId,
                    data.GameProviderId,
                    data.PlayerCode,
                    data.GameId,
                    data.CurrencyCode,
                    data.Status,
                    data.GameProviderConfigurationId,
                    data.WalletId,
                    data.CreatedOn,
                    data.ClosedOn,
                    data.UpdatedOn,
                    data.BrandId,
                    data.ExternalCreatedOn,
                    data.ExternalClosedOn,
                    data.BetAmount,
                    data.ValidBetAmount,
                    data.SettleAmount,
                    data.Commission,
                    data.Turnover,
                    data.WinLoss,
                    data.TieBetAdjustment,
                    data.ClientStatus,
                    data.AdditionalData)).Returns(ValueTask.FromResult(expected));
            _ = fakeGameRound.GetAsync(expected).Returns(ValueTask.FromResult(gameRound));

            await service.ExecuteAsync(data).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Is(gameRound));
        }
    }
}
