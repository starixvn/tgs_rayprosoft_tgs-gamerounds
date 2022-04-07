using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories.UnitTest
{
    [TestClass]
    public class GameRoundRepositoryUnitTest
    {
        [TestMethod]
        public async Task Add_Round()
        {
            var fakeLogger = NullLogger<GameRoundRepository>.Instance;
            var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
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

            var data = new AddRound(
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

            var fakeRoundRepository = new GameRoundRepository(context, fakeLogger, fakeSerialNumberGenerator);

            _ = await fakeRoundRepository.AddRoundAsync(data).ConfigureAwait(false);

            var actual = await fakeRoundRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(gameProviderId, actual.GameProviderId);
            Assert.AreEqual(externalRoundId, actual.ExternalRoundId);
            Assert.AreEqual(gameId, actual.GameId);
            Assert.AreEqual(playerCode, actual.PlayerCode);
        }

        [TestMethod]
        public async Task Get_Round()
        {
            var fakeLogger = NullLogger<GameRoundRepository>.Instance;
            var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
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

            var fakeRoundRepository = new GameRoundRepository(context, fakeLogger, fakeSerialNumberGenerator);

            var actual = await fakeRoundRepository.GetAsync(roundId).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(roundId, actual.RoundId);
        }

        [TestMethod]
        public async Task Get_Round_ExternalRoundId()
        {
            var fakeLogger = NullLogger<GameRoundRepository>.Instance;
            var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
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

            var fakeRoundRepository = new GameRoundRepository(context, fakeLogger, fakeSerialNumberGenerator);

            var actual = await fakeRoundRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(roundId, actual.RoundId);
            Assert.AreEqual(gameProviderId, actual.GameProviderId);
            Assert.AreEqual(externalRoundId, actual.ExternalRoundId);
            Assert.AreEqual(gameId, actual.GameId);
            Assert.AreEqual(playerCode, actual.PlayerCode);
        }

        [TestMethod]
        public async Task Update_Round_Closed()
        {
            var fakeLogger = NullLogger<GameRoundRepository>.Instance;
            var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
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
                ClosedOn = null,
                UpdatedOn = DateTime.UtcNow,
                BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn = DateTime.UtcNow,
                ExternalClosedOn = null,
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

            var fakeRoundRepository = new GameRoundRepository(context, fakeLogger, fakeSerialNumberGenerator);

            var oldActual = await fakeRoundRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);
            Assert.IsNotNull(oldActual);
            Assert.AreEqual(roundId, oldActual.RoundId);
            Assert.AreEqual(gameProviderId, oldActual.GameProviderId);
            Assert.AreEqual(externalRoundId, oldActual.ExternalRoundId);
            Assert.AreEqual(gameId, oldActual.GameId);
            Assert.AreEqual(playerCode, oldActual.PlayerCode);
            Assert.AreEqual(GameRoundStatus.Open, oldActual.Status);
            Assert.AreEqual(GameRoundClientStatus.Open, oldActual.ClientStatus);

            var data = new UpdateRoundClosed(
                RoundId: roundId,
                ClosedOn: DateTimeOffset.UtcNow,
                UpdatedOn: DateTimeOffset.UtcNow,
                ExternalClosedOn: DateTimeOffset.UtcNow,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                SettleAmount: 0,
                Commission: 0,
                Turnover: 10m,
                WinLoss: -10m,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Loss,
                AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

            await fakeRoundRepository.UpdateRoundClosedAsync(data).ConfigureAwait(false);

            var actual = await fakeRoundRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(roundId, actual.RoundId);
            Assert.AreEqual(gameProviderId, actual.GameProviderId);
            Assert.AreEqual(externalRoundId, actual.ExternalRoundId);
            Assert.AreEqual(gameId, actual.GameId);
            Assert.AreEqual(playerCode, actual.PlayerCode);
            Assert.AreEqual(GameRoundClientStatus.Loss, actual.ClientStatus);
            Assert.AreEqual(10m, actual.WinLoss);
        }

        [TestMethod]
        public async Task Update_Round_Open()
        {
            var fakeLogger = NullLogger<GameRoundRepository>.Instance;
            var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
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
                BetAmount = 10.000000m,
                ValidBetAmount = 10.000000m,
                SettleAmount = null,
                Commission = null,
                Turnover = 10.000000m,
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

            var data = new UpdateRoundOpen(
                RoundId: roundId,
                UpdatedOn: DateTimeOffset.UtcNow,
                BetAmount: 20m,
                ValidBetAmount: 20m,
                Commission: 0,
                Turnover: 20m,
                AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

            var fakeRoundRepository = new GameRoundRepository(context, fakeLogger, fakeSerialNumberGenerator);

            await fakeRoundRepository.UpdateRoundOpenAsync(data).ConfigureAwait(false);

            var actual = await fakeRoundRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(roundId, actual.RoundId);
            Assert.AreEqual(gameProviderId, actual.GameProviderId);
            Assert.AreEqual(externalRoundId, actual.ExternalRoundId);
            Assert.AreEqual(gameId, actual.GameId);
            Assert.AreEqual(playerCode, actual.PlayerCode);
            Assert.AreEqual(data.BetAmount, actual.BetAmount);
            Assert.AreEqual(data.ValidBetAmount, actual.ValidBetAmount);
            Assert.AreEqual(data.Turnover, actual.Turnover);
            Assert.AreEqual(GameRoundClientStatus.Open, actual.ClientStatus);
            Assert.AreEqual(null, actual.ReopenTurnoverAdjustment);
            Assert.AreEqual(null, actual.ReopenedOn);
        }

        [TestMethod]
        public async Task Update_Round_Reopen()
        {
            var fakeLogger = NullLogger<GameRoundRepository>.Instance;
            var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
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
                Status = (short)GameRoundStatus.Closed,
                GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId = walletId,
                CreatedOn = DateTime.UtcNow,
                ClosedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn = DateTime.UtcNow,
                ExternalClosedOn = DateTime.UtcNow,
                BetAmount = 10m,
                ValidBetAmount = 10m,
                SettleAmount = 0m,
                Commission = 0m,
                Turnover = 10m,
                WinLoss = 10m,
                TieBetAdjustment = null,
                ClientStatus = (short)GameRoundClientStatus.Loss,
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

            var fakeRoundRepository = new GameRoundRepository(context, fakeLogger, fakeSerialNumberGenerator);

            var oldActual = await fakeRoundRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);
            Assert.IsNotNull(oldActual);
            Assert.AreEqual(roundId, oldActual.RoundId);
            Assert.AreEqual(gameProviderId, oldActual.GameProviderId);
            Assert.AreEqual(externalRoundId, oldActual.ExternalRoundId);
            Assert.AreEqual(gameId, oldActual.GameId);
            Assert.AreEqual(playerCode, oldActual.PlayerCode);
            Assert.AreEqual(GameRoundClientStatus.Loss, oldActual.ClientStatus);
            Assert.AreEqual(0m, oldActual.SettleAmount);
            Assert.AreEqual(10m, oldActual.WinLoss);
            Assert.IsNull(oldActual.ReopenedOn);
            Assert.IsNull(oldActual.ReopenTurnoverAdjustment);

            var data = new UpdateRoundReopen(
                RoundId: roundId,
                Status: GameRoundStatus.Closed,
                ClosedOn: DateTimeOffset.UtcNow,
                UpdatedOn: DateTimeOffset.UtcNow,
                ExternalClosedOn: DateTimeOffset.UtcNow,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                SettleAmount: 20m,
                Commission: 0,
                Turnover: 10m,
                WinLoss: 10m,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Win,
                AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

            await fakeRoundRepository.UpdateRoundReopenAsync(data).ConfigureAwait(false);

            var actual = await fakeRoundRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(GameRoundClientStatus.Win, actual.ClientStatus);
            Assert.AreEqual(20m, actual.SettleAmount);
            Assert.AreEqual(-10m, actual.WinLoss);
            Assert.IsNotNull(actual.ReopenedOn);
            Assert.AreEqual(10m, actual.ReopenTurnoverAdjustment);
        }
    }
}
