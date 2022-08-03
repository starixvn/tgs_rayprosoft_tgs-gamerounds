using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.Models;
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

            var data = new AddGameRoundRecord(
                RoundId: Guid.NewGuid(),
                ExternalRoundId: Guid.NewGuid().ToString("N"),
                ExternalGameId: "Bacc",
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "KY",
                GameProviderConfigurationId: Guid.NewGuid(),
                GameId: 1,
                BrandId: Guid.NewGuid(),
                PlayerCode: "test",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "CNY",
                Status: GameRoundStatus.Open,
                ClientStatus: GameRoundClientStatus.Open,
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: null,
                UpdatedOn: DateTimeOffset.UtcNow,
                ExternalCreatedOn: null,
                ExternalClosedOn: null,
                Bet: 0,
                ValidBet: 0,
                Settle: 0,
                Commission: 0,
                Turnover: 0,
                CompanyWinLoss: 0,
                TieBetAdjustment: 0,
                ReopenTurnoverAdjustment: 0,
                ReopenedOn: null);

            var expected = Guid.NewGuid();

            _ = fakeSerialNumberGenerator.GenerateAsync().Returns(ValueTask.FromResult(expected));

            var repository = new GameRoundRecordRepository(fakeMongoDatabase, fakeLogger, fakeSerialNumberGenerator);

            await repository.AddAsync(data).ConfigureAwait(false);
        }
    }
}
