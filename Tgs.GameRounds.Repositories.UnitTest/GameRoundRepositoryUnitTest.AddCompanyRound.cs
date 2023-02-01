using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories.UnitTest;

[TestClass]
public class GameRoundRepositoryUnitTestAddCompanyRound
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<GameRoundRepository>.Instance;
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var options = new DbContextOptionsBuilder<BetContext>()
           .UseInMemoryDatabase(databaseName: "bet")
           .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
           .Options;
        var context = new BetContext(options);
        _ = context.Database.EnsureDeleted();

        var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
        var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
        var gameId = 500001;
        var playerCode = "4ues8xikza7o";
        var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

        var data = new AddCompanyRound(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            PlayerCode: playerCode,
            GameId: gameId,
            CurrencyCode: "CNY",
            GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
            WalletId: walletId,
            BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
            ExternalCreatedOn: DateTimeOffset.UtcNow,
            ExternalClosedOn: DateTimeOffset.UtcNow,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0,
            Turnover: 10m,
            CompanyWinLoss: -10m,
            ClientStatus: GameRoundClientStatus.Win,
            OperatorId: Guid.NewGuid(),
            Remark: "Test");

        var fakeRoundRepository = new GameRoundRepository(
            context,
            fakeLogger,
            fakeSerialNumberGenerator);

        _ = await fakeRoundRepository.AddCompanyRoundAsync(data).ConfigureAwait(false);

        var actual = await fakeRoundRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);
        Assert.IsNotNull(actual);
        Assert.AreEqual(gameProviderId, actual.GameProviderId);
        Assert.AreEqual(externalRoundId, actual.ExternalRoundId);
        Assert.AreEqual(gameId, actual.GameId);
        Assert.AreEqual(playerCode, actual.PlayerCode);
        Assert.IsNotNull(actual.ClosedOn);
        Assert.IsNotNull(actual.ExternalClosedOn);
        Assert.AreEqual(data.ClientStatus, actual.ClientStatus);
        Assert.AreEqual(data.BetAmount, actual.BetAmount);
        Assert.AreEqual(data.ValidBetAmount, actual.ValidBetAmount);
        Assert.AreEqual(data.SettleAmount, actual.SettleAmount);
        Assert.AreEqual(data.Commission, actual.Commission);
        Assert.AreEqual(data.Turnover, actual.Turnover);
        Assert.AreEqual(data.CompanyWinLoss, actual.CompanyWinLoss);
    }
}
