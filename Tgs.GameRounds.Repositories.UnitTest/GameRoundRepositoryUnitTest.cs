using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories.UnitTest;

[TestClass]
public class GameRoundRepositoryUnitTest
{
    [TestMethod]
    public async Task Get_Round()
    {
        var fakeLogger = NullLogger<GameRoundRepository>.Instance;
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        var fakeGameRound = Substitute.For<IGameRound>();
        var options = new DbContextOptionsBuilder<BetContext>()
           .UseInMemoryDatabase(databaseName: "bet")
           .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
           .Options;
        var context = new BetContext(options);
        _ = context.Database.EnsureDeleted();

        var roundId = new Guid("cc4d6693-417d-401a-8ccf-68f25382a134");

        _ = await context.Rounds.AddAsync(new Round()
        {
            Id = roundId,
            ExternalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134",
            GameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
            PlayerCode = "4ues8xikza7o",
            GameId = 500001,
            CurrencyCode = "CNY",
            Status = (short)GameRoundStatus.Open,
            GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
            WalletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
            CreatedOn = DateTime.UtcNow,
            ClosedOn = (DateTime?)null,
            UpdatedOn = DateTime.UtcNow,
            BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
            ExternalCreatedOn = DateTime.UtcNow,
            ExternalClosedOn = (DateTime?)null,
            BetAmount = 10m,
            ValidBetAmount = 10m,
            SettleAmount = null,
            Commission = null,
            Turnover = 10m,
            WinLoss = null,
            TieBetAdjustment = null,
            ClientStatus = (short)GameRoundClientStatus.Open,
            ReopenTurnoverAdjustment = null,
            ReopenedOn = (DateTime?)null,
            EffectiveBettingPercentage = null,
        }).ConfigureAwait(false);

        _ = await context.RoundDetails.AddAsync(new RoundDetail()
        {
            RoundId = roundId,
            AdditionalData = JsonSerializer.Serialize(new { RoundId = roundId })
        }).ConfigureAwait(false);

        _ = await context.SaveChangesAsync().ConfigureAwait(false);

        var fakeRoundRepository = new GameRoundRepository(
            context,
            fakeLogger,
            fakeSerialNumberGenerator);

        var actual = await fakeRoundRepository.GetAsync(roundId).ConfigureAwait(false);
        Assert.IsNotNull(actual);
        Assert.AreEqual(roundId, actual.RoundId);
    }

    [TestMethod]
    public async Task Get_Round_ExternalRoundId()
    {
        var fakeLogger = NullLogger<GameRoundRepository>.Instance;
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        var fakeGameRound = Substitute.For<IGameRound>();
        var options = new DbContextOptionsBuilder<BetContext>()
           .UseInMemoryDatabase(databaseName: "bet")
           .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
           .Options;
        var context = new BetContext(options);
        _ = context.Database.EnsureDeleted();

        var roundId = Guid.NewGuid();
        var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
        var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
        var gameId = 500001;
        var playerCode = "4ues8xikza7o";
        var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

        _ = await context.Rounds.AddAsync(new Round()
        {
            Id = roundId,
            ExternalRoundId = externalRoundId,
            GameProviderId = gameProviderId,
            PlayerCode = playerCode,
            GameId = gameId,
            CurrencyCode = "CNY",
            Status = (short)GameRoundStatus.Open,
            GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
            WalletId = walletId,
            CreatedOn = DateTime.UtcNow,
            ClosedOn = (DateTime?)null,
            UpdatedOn = DateTime.UtcNow,
            BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
            ExternalCreatedOn = DateTime.UtcNow,
            ExternalClosedOn = (DateTime?)null,
            BetAmount = 10m,
            ValidBetAmount = 10m,
            SettleAmount = null,
            Commission = null,
            Turnover = 10m,
            WinLoss = null,
            TieBetAdjustment = null,
            ClientStatus = (short)GameRoundClientStatus.Open,
            ReopenTurnoverAdjustment = null,
            ReopenedOn = null,
            EffectiveBettingPercentage = null,
        }).ConfigureAwait(false);

        _ = await context.RoundDetails.AddAsync(new RoundDetail()
        {
            RoundId = roundId,
            AdditionalData = JsonSerializer.Serialize(new { RoundId = roundId })
        }).ConfigureAwait(false);

        _ = await context.SaveChangesAsync().ConfigureAwait(false);

        var fakeRoundRepository = new GameRoundRepository(
            context,
            fakeLogger,
            fakeSerialNumberGenerator);

        var actual = await fakeRoundRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);
        Assert.IsNotNull(actual);
        Assert.AreEqual(roundId, actual.RoundId);
        Assert.AreEqual(gameProviderId, actual.GameProviderId);
        Assert.AreEqual(externalRoundId, actual.ExternalRoundId);
        Assert.AreEqual(gameId, actual.GameId);
        Assert.AreEqual(playerCode, actual.PlayerCode);
    }
}
