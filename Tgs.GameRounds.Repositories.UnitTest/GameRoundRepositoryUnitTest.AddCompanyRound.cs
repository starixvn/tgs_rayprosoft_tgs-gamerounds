using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories.UnitTest;

public class GameRoundRepositoryUnitTestAddCompanyRound
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<GameRoundRepository>.Instance;
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        var fakeGameRound = Substitute.For<IGameRoundRepository>();
        var options = new DbContextOptionsBuilder<BetContext>()
           .UseInMemoryDatabase(databaseName: "bet_add_company_round")
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
        Assert.NotNull(actual);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(externalRoundId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(playerCode, actual.PlayerCode);
        Assert.NotNull(actual.ClosedOn);
        Assert.NotNull(actual.ExternalClosedOn);
        Assert.Equal(data.ClientStatus, actual.ClientStatus);
        Assert.Equal(data.BetAmount, actual.BetAmount);
        Assert.Equal(data.ValidBetAmount, actual.ValidBetAmount);
        Assert.Equal(data.SettleAmount, actual.SettleAmount);
        Assert.Equal(data.Commission, actual.Commission);
        Assert.Equal(data.Turnover, actual.Turnover);
        Assert.Equal(data.CompanyWinLoss, actual.CompanyWinLoss);
    }
}
