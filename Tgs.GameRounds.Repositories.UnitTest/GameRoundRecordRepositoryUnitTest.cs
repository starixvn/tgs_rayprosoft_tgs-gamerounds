using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories.UnitTest
{
    [TestClass]
    public class GameRoundRecordRepositoryUnitTest
    {
        [TestMethod]
        public async Task Add_BetGameCash()
        {
            var fakeLogger = NullLogger<GameRoundRecordRepository>.Instance;
            var fakeMongoDatabase = Substitute.For<IMongoDatabase>();
            var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

            var data = new GameRoundInfo(
                RoundId: Guid.NewGuid(),
                ExternalRoundId: Guid.NewGuid().ToString("N"),
                GameProviderId: Guid.NewGuid(),
                PlayerCode: "test",
                GameId: 1,
                CurrencyCode: "CNY",
                Status: GameRoundStatus.Open,
                GameProviderConfigurationId: Guid.NewGuid(),
                WalletId: Guid.NewGuid(),
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: null,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: Guid.NewGuid(),
                ExternalCreatedOn: null,
                ExternalClosedOn: null,
                BetAmount: null,
                ValidBetAmount: null,
                SettleAmount: null,
                Commission: null,
                Turnover: null,
                CompanyWinLoss: null,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Open,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                EffectiveBettingPercentage: null,
                AdditionalData: string.Empty);
            var expected = Guid.NewGuid();
            _ = fakeSerialNumberGenerator.GenerateAsync().Returns(ValueTask.FromResult(expected));

            var repository = new GameRoundRecordRepository(fakeMongoDatabase, fakeLogger, fakeSerialNumberGenerator);

            await repository.AddAsync(data).ConfigureAwait(false);
        }
    }
}
