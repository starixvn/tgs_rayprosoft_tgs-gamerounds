using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.UnitTest
{
    [TestClass]
    public class UpdateRoundCommandServiceUnitTest
    {
        [TestMethod]
        public async Task ExternalRoundId_Not_Exist()
        {
            var fakeGameRoundCache = Substitute.For<IGameRoundCacheService>();
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRoundRepository>();

            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var data = new UpdateRoundQuery(
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
                RoundId: data.RoundId,
                ExternalRoundId: externalRoundId,
                GameProviderId: gameProviderId,
                PlayerCode: playerCode,
                GameId: gameId,
                CurrencyCode: "CNY",
                Status: GameRoundStatus.Open,
                GameProviderConfigurationId: Guid.NewGuid(),
                WalletId: Guid.NewGuid(),
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: data.ClosedOn,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: Guid.NewGuid(),
                ExternalCreatedOn: DateTimeOffset.UtcNow,
                ExternalClosedOn: data.ExternalClosedOn,
                BetAmount: data.BetAmount,
                ValidBetAmount: data.ValidBetAmount,
                SettleAmount: data.SettleAmount,
                Commission: data.Commission,
                Turnover: data.Turnover,
                WinLoss: data.WinLoss,
                TieBetAdjustment: data.TieBetAdjustment,
                ClientStatus: GameRoundClientStatus.Open,
                ReopenTurnoverAdjustment: data.ReopenTurnoverAdjustment,
                ReopenedOn: data.ReopenedOn,
                EffectiveBettingPercentage: null,
                AdditionalData: data.AdditionalData ?? string.Empty);

            var expected = new UpdateRoundResult(false);

            var repository = new UpdateRoundCommandService(
                fakeGameRoundCache,
                fakeGameRoundRecord,
                fakeGameRound);

            _ = fakeGameRoundCache.ExistAsync(data.RoundId).Returns(ValueTask.FromResult(expected.Success));

            var actual = await repository.GetAsync(data).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Success, actual.Success);

            _ = fakeGameRoundRecord.Received(0).AddAsync(Arg.Is(gameRound));
        }

        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameRoundCache = Substitute.For<IGameRoundCacheService>();
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRoundRepository>();

            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var data = new UpdateRoundQuery(
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
                RoundId: data.RoundId,
                ExternalRoundId: externalRoundId,
                GameProviderId: gameProviderId,
                PlayerCode: playerCode,
                GameId: gameId,
                CurrencyCode: "CNY",
                Status: GameRoundStatus.Open,
                GameProviderConfigurationId: Guid.NewGuid(),
                WalletId: Guid.NewGuid(),
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: data.ClosedOn,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: Guid.NewGuid(),
                ExternalCreatedOn: DateTimeOffset.UtcNow,
                ExternalClosedOn: data.ExternalClosedOn,
                BetAmount: data.BetAmount,
                ValidBetAmount: data.ValidBetAmount,
                SettleAmount: data.SettleAmount,
                Commission: data.Commission,
                Turnover: data.Turnover,
                WinLoss: data.WinLoss,
                TieBetAdjustment: data.TieBetAdjustment,
                ClientStatus: GameRoundClientStatus.Open,
                ReopenTurnoverAdjustment: data.ReopenTurnoverAdjustment,
                ReopenedOn: data.ReopenedOn,
                EffectiveBettingPercentage: null,
                AdditionalData: data.AdditionalData ?? string.Empty);

            var expected = new UpdateRoundResult(true);

            var repository = new UpdateRoundCommandService(
                fakeGameRoundCache,
                fakeGameRoundRecord,
                fakeGameRound);

            _ = fakeGameRoundCache.ExistAsync(data.RoundId).Returns(ValueTask.FromResult<bool>(expected.Success));

            _ = fakeGameRound.UpdateRoundAsync(data);

            _ = fakeGameRound.GetAsync(data.RoundId).Returns(ValueTask.FromResult(gameRound));

            var actual = await repository.GetAsync(data).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Equals(expected, actual);
            Assert.AreEqual(expected.Success, actual.Success);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Is(gameRound));
        }
    }
}
