using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.IntegrationTests
{
    [TestClass]
    public partial class GameRoundsClientUnitTest
    {
        [TestMethod]
        public async Task GameRounds_GetGameRound()
        {
            var roundId = Guid.NewGuid();
            var expected = GameRoundStatus.Open;
            var externalGameId = "Bacc";
            var gameProviderCode = "DG";
            var data = new GameRoundInfo(
               RoundId: roundId,
               ExternalRoundId: Guid.NewGuid().ToString("N"),
               GameProviderId: Guid.NewGuid(),
               PlayerCode: "test",
               GameId: 1,
               CurrencyCode: "CNY",
               Status: expected,
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
               WinLoss: null,
               TieBetAdjustment: null,
               ClientStatus: GameRoundClientStatus.Open,
               ReopenTurnoverAdjustment: null,
               ReopenedOn: null,
               EffectiveBettingPercentage: null,
               AdditionalData: string.Empty);

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeRepository = Substitute.For<IGameRoundRepository>();

                _ = fakeRepository.GetAsync(roundId).Returns(ValueTask.FromResult(data));


                _ = services.AddSingleton(fakeRepository);

                var fakeGame = Substitute.For<IGame>();

                _ = fakeGame.GetExternalGameIdAsync(data.GameId).Returns(ValueTask.FromResult(externalGameId));

                _ = services.AddSingleton(fakeGame);


                var fakeGameProvider = Substitute.For<IGameProvider>();

                _ = fakeGameProvider.GetGameProviderCodeAsync(data.GameProviderId).Returns(ValueTask.FromResult(gameProviderCode));

                _ = services.AddSingleton(fakeGameProvider);

                _ = services.AddFakeNatsMessageQueue();
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.GetGameRoundAsync(roundId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(roundId, actual.RoundId);
            Assert.AreEqual(externalGameId, actual.ExternalGameId);
            Assert.AreEqual(gameProviderCode, actual.GameProviderCode);
        }

        [TestMethod]
        public async Task GameRounds_GetGameRoundByExternalGame()
        {
            var roundId = Guid.NewGuid();
            Guid gameProviderId = Guid.NewGuid();
            string externalRoundId = Guid.NewGuid().ToString("N");
            string externalGameId = "bacc";
            var gameProviderCode = "DG";
            int gameId = 1;
            Guid walletId = Guid.NewGuid();
            var data = new GameRoundInfo(
              RoundId: roundId,
              ExternalRoundId: externalRoundId,
              GameProviderId: gameProviderId,
              PlayerCode: "test",
              GameId: gameId,
              CurrencyCode: "CNY",
              Status: GameRoundStatus.Open,
              GameProviderConfigurationId: Guid.NewGuid(),
              WalletId: walletId,
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
              WinLoss: null,
              TieBetAdjustment: null,
              ClientStatus: GameRoundClientStatus.Open,
              ReopenTurnoverAdjustment: null,
              ReopenedOn: null,
              EffectiveBettingPercentage: null,
              AdditionalData: string.Empty);

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeGame = Substitute.For<IGame>();

                _ = fakeGame.GetGameIdAsync(gameProviderId, externalGameId).Returns(ValueTask.FromResult<int>(gameId));

                _ = services.AddSingleton(fakeGame);


                var fakeGameProvider = Substitute.For<IGameProvider>();

                _ = fakeGameProvider.GetGameProviderCodeAsync(data.GameProviderId).Returns(ValueTask.FromResult(gameProviderCode));

                _ = services.AddSingleton(fakeGameProvider);


                var fakeGameRound = Substitute.For<IGameRoundRepository>();

                _ = fakeGameRound.GetAsync(gameProviderId, externalRoundId, gameId, walletId).Returns(ValueTask.FromResult<GameRoundInfo?>(data));

                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddFakeNatsMessageQueue();
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.GetGameRoundAsync(gameProviderId, externalRoundId, externalGameId, walletId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(roundId, actual.RoundId);
            Assert.AreEqual(externalGameId, actual.ExternalGameId);
            Assert.AreEqual(gameProviderCode, actual.GameProviderCode);
        }

        [TestMethod]
        public async Task GameRounds_GetGameRoundByGame()
        {
            var roundId = Guid.NewGuid();
            Guid gameProviderId = Guid.NewGuid();
            string externalRoundId = Guid.NewGuid().ToString("N");
            var externalGameId = "Bacc";
            var gameProviderCode = "DG";
            int gameId = 1;
            Guid walletId = Guid.NewGuid();
            var data = new GameRoundInfo(
              RoundId: roundId,
              ExternalRoundId: externalRoundId,
              GameProviderId: gameProviderId,
              PlayerCode: "test",
              GameId: gameId,
              CurrencyCode: "CNY",
              Status: GameRoundStatus.Open,
              GameProviderConfigurationId: Guid.NewGuid(),
              WalletId: walletId,
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
              WinLoss: null,
              TieBetAdjustment: null,
              ClientStatus: GameRoundClientStatus.Open,
              ReopenTurnoverAdjustment: null,
              ReopenedOn: null,
              EffectiveBettingPercentage: null,
              AdditionalData: string.Empty);

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeRepository = Substitute.For<IGameRoundRepository>();

                _ = fakeRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).Returns(ValueTask.FromResult<GameRoundInfo?>(data));

                _ = services.AddSingleton(fakeRepository);


                var fakeGame = Substitute.For<IGame>();

                _ = fakeGame.GetExternalGameIdAsync(data.GameId).Returns(ValueTask.FromResult(externalGameId));

                _ = services.AddSingleton(fakeGame);


                var fakeGameProvider = Substitute.For<IGameProvider>();

                _ = fakeGameProvider.GetGameProviderCodeAsync(data.GameProviderId).Returns(ValueTask.FromResult(gameProviderCode));

                _ = services.AddSingleton(fakeGameProvider);

                _ = services.AddFakeNatsMessageQueue();
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.GetGameRoundAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(roundId, actual.RoundId);
            Assert.AreEqual(externalGameId, actual.ExternalGameId);
            Assert.AreEqual(gameProviderCode, actual.GameProviderCode);

        }

        [TestMethod]
        public async Task GameRounds_GetStatus()
        {
            var roundId = Guid.NewGuid();
            var expected = GameRoundStatus.Open;
            var data = new GameRoundInfo(
               RoundId: roundId,
               ExternalRoundId: Guid.NewGuid().ToString("N"),
               GameProviderId: Guid.NewGuid(),
               PlayerCode: "test",
               GameId: 1,
               CurrencyCode: "CNY",
               Status: expected,
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
               WinLoss: null,
               TieBetAdjustment: null,
               ClientStatus: GameRoundClientStatus.Open,
               ReopenTurnoverAdjustment: null,
               ReopenedOn: null,
               EffectiveBettingPercentage: null,
               AdditionalData: string.Empty);

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeRepository = Substitute.For<IGameRoundRepository>();

                _ = fakeRepository.GetAsync(roundId).Returns(ValueTask.FromResult(data));

                _ = services.AddSingleton(fakeRepository);

                _ = services.AddFakeNatsMessageQueue();
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.GetStatusAsync(roundId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task GameRounds_NewRound()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var expected = new NewRoundResultViewModel(true, Guid.NewGuid());
            var data = new NewRoundQueryViewModel()
            {
                ExternalRoundId = externalRoundId,
                GameProviderId = gameProviderId,
                PlayerCode = playerCode,
                GameId = gameId,
                CurrencyCode = "CNY",
                GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
                BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn = DateTimeOffset.UtcNow
            };

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeGameRoundCache = Substitute.For<IGameRoundService>();
                _ = fakeGameRoundCache.GetRoundIdAsync(data.GameProviderId, data.ExternalRoundId, data.GameId, data.WalletId).Returns(ValueTask.FromResult<Guid?>(null));
                _ = services.AddSingleton(fakeGameRoundCache);

                var fakeGameRound = Substitute.For<IGameRoundRepository>();

                _ = fakeGameRound.NewRoundAsync(new(
                    data.ExternalRoundId,
                    data.GameProviderId,
                    data.PlayerCode,
                    data.GameId,
                    data.CurrencyCode,
                    data.GameProviderConfigurationId,
                    data.WalletId,
                    data.BrandId,
                    data.ExternalCreatedOn)).Returns(ValueTask.FromResult(expected.RoundId));

                var gameRound = new GameRoundInfo(
                    RoundId: expected.RoundId!.Value,
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
                   AdditionalData: string.Empty);

                _ = fakeGameRound.GetAsync(expected.RoundId.Value).Returns(ValueTask.FromResult(gameRound));

                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddSingleton(fakeGameRoundRecord);

                _ = services.AddFakeNatsMessageQueue();
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.NewRoundAsync(data).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            _ = Equals(expected, actual);
            Assert.AreEqual(expected.Success, actual.Success);
            Assert.AreEqual(expected.RoundId, actual.RoundId);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Any<GameRoundInfo>());
        }

        [TestMethod]
        public async Task GameRounds_UpdateRound()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var data = new UpdateRoundQueryViewModel()
            {
                Status = GameRoundStatus.Closed,
                ClosedOn = DateTimeOffset.UtcNow,
                ExternalClosedOn = DateTimeOffset.UtcNow,
                BetAmount = 20m,
                ValidBetAmount = 20m,
                SettleAmount = 40m,
                Commission = 0m,
                Turnover = 20m,
                WinLoss = -20m,
                TieBetAdjustment = null,
                ClientStatus = GameRoundClientStatus.Win,
                ReopenTurnoverAdjustment = null,
                ReopenedOn = null,
                AdditionalData = null
            };

            var expected = new UpdateRoundResultViewModel(true);

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeGameRoundCache = Substitute.For<IGameRoundService>();
                _ = fakeGameRoundCache.ExistAsync(roundId).Returns(ValueTask.FromResult(true));
                _ = services.AddSingleton(fakeGameRoundCache);

                var fakeGameRound = Substitute.For<IGameRoundRepository>();

                _ = fakeGameRound.UpdateRoundAsync(new(
                    roundId,
                    data.Status,
                    data.ClosedOn,
                    data.ExternalClosedOn,
                    data.BetAmount,
                    data.ValidBetAmount,
                    data.SettleAmount,
                    data.Commission,
                    data.Turnover,
                    data.WinLoss,
                    data.TieBetAdjustment,
                    data.ClientStatus,
                    data.ReopenTurnoverAdjustment,
                    data.ReopenedOn,
                    data.AdditionalData));

                var gameRound = new GameRoundInfo(
                    RoundId: roundId,
                    ExternalRoundId: externalRoundId,
                    GameProviderId: gameProviderId,
                    PlayerCode: playerCode,
                    GameId: gameId,
                    CurrencyCode: "CNY",
                    Status: GameRoundStatus.Open,
                    GameProviderConfigurationId: Guid.NewGuid(),
                    WalletId: Guid.NewGuid(),
                    CreatedOn: DateTimeOffset.UtcNow,
                    ClosedOn: null,
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

                _ = fakeGameRound.GetAsync(roundId).Returns(ValueTask.FromResult(gameRound));

                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddSingleton(fakeGameRoundRecord);

                _ = services.AddFakeNatsMessageQueue();
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            var actual = await sut.UpdateRoundAsync(roundId, data).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            _ = Equals(expected, actual);
            Assert.AreEqual(expected.Success, actual.Success);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Any<GameRoundInfo>());
        }

        [TestMethod]
        public async Task GameRounds_UpdateRoundClosed()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var data = new UpdateRoundClosedViewModel(
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

            var expected = new UpdateRoundResultViewModel(true);

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeGameRoundCache = Substitute.For<IGameRoundService>();
                _ = fakeGameRoundCache.ExistAsync(roundId).Returns(ValueTask.FromResult(true));
                _ = services.AddSingleton(fakeGameRoundCache);

                var fakeGameRound = Substitute.For<IGameRoundRepository>();

                _ = fakeGameRound.UpdateRoundClosedAsync(new(
                    roundId,
                    data.ClosedOn,
                    data.UpdatedOn,
                    data.ExternalClosedOn,
                    data.BetAmount,
                    data.ValidBetAmount,
                    data.SettleAmount,
                    data.Commission,
                    data.Turnover,
                    data.WinLoss,
                    data.TieBetAdjustment,
                    data.ClientStatus,
                    data.AdditionalData));

                var gameRound = new GameRoundInfo(
                    RoundId: roundId,
                    ExternalRoundId: externalRoundId,
                    GameProviderId: gameProviderId,
                    PlayerCode: playerCode,
                    GameId: gameId,
                    CurrencyCode: "CNY",
                    Status: GameRoundStatus.Open,
                    GameProviderConfigurationId: Guid.NewGuid(),
                    WalletId: Guid.NewGuid(),
                    CreatedOn: DateTimeOffset.UtcNow,
                    ClosedOn: null,
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
                    ReopenTurnoverAdjustment: null,
                    ReopenedOn: null,
                    EffectiveBettingPercentage: null,
                    AdditionalData: data.AdditionalData ?? string.Empty);

                _ = fakeGameRound.GetAsync(roundId).Returns(ValueTask.FromResult(gameRound));

                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddSingleton(fakeGameRoundRecord);

                _ = services.AddFakeNatsMessageQueue();
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            await sut.UpdateRoundClosedAsync(roundId, data).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Any<GameRoundInfo>());
        }

        [TestMethod]
        public async Task GameRounds_UpdateRoundDetail()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var data = new UpdateRoundDetailViewModel(
                AdditionalData: "{\"id\":10531704494,\"tableId\":10102,\"shoeId\":17,\"playId\":34,\"lobbyId\":1,\"gameType\":1,\"gameId\":1,\"memberId\":73304209,\"parentId\":19377,\"betTime\":\"2022-03-1709:19:38\",\"calTime\":\"2022-03-1709:20:01\",\"winOrLoss\":40.0,\"balanceBefore\":6020.41,\"betPoints\":20.0,\"betPointsz\":0.0,\"availableBet\":20.0,\"userName\":\"4UDW8YGX311V\",\"result\":\"{\\u0022result\\u0022:\\u00225,2,7\\u0022,\\u0022poker\\u0022:{\\u0022banker\\u0022:\\u002238-36-36\\u0022,\\u0022player\\u0022:\\u002246-36-0\\u0022}}\",\"betDetail\":\"{\\u0022player\\u0022:20.0,\\u0022playerW\\u0022:40.0}\",\"ip\":\"220.132.206.70\",\"ext\":\"220317B020851\",\"isRevocation\":1,\"currencyId\":1,\"deviceType\":5,\"serial\":\"-10531704494\",\"roadid\":0,\"pluginid\":0}"
                );
            var expected = new UpdateRoundResultViewModel(true);

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeGameRoundCache = Substitute.For<IGameRoundService>();
                _ = fakeGameRoundCache.ExistAsync(roundId).Returns(ValueTask.FromResult(true));
                _ = services.AddSingleton(fakeGameRoundCache);

                var fakeGameRound = Substitute.For<IGameRoundRepository>();

                _ = fakeGameRound.UpdateRoundDetailAsync(
                    roundId,
                    data.AdditionalData);

                var gameRound = new GameRoundInfo(
                    RoundId: roundId,
                    ExternalRoundId: externalRoundId,
                    GameProviderId: gameProviderId,
                    PlayerCode: playerCode,
                    GameId: gameId,
                    CurrencyCode: "CNY",
                    Status: GameRoundStatus.Open,
                    GameProviderConfigurationId: Guid.NewGuid(),
                    WalletId: Guid.NewGuid(),
                    CreatedOn: DateTimeOffset.UtcNow,
                    ClosedOn: null,
                    UpdatedOn: DateTimeOffset.UtcNow,
                    BrandId: Guid.NewGuid(),
                    ExternalCreatedOn: DateTimeOffset.UtcNow,
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
                    AdditionalData: data.AdditionalData ?? string.Empty);

                _ = fakeGameRound.GetAsync(roundId).Returns(ValueTask.FromResult(gameRound));

                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddSingleton(fakeGameRoundRecord);

                _ = services.AddFakeNatsMessageQueue();
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            await sut.UpdateRoundDetailAsync(roundId, data).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Any<GameRoundInfo>());
        }

        [TestMethod]
        public async Task GameRounds_UpdateRoundOpen()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var data = new UpdateRoundOpenViewModel(
                UpdatedOn: DateTimeOffset.UtcNow,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                Commission: 0,
                Turnover: 10m,
                AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

            var expected = new UpdateRoundResultViewModel(true);

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeGameRoundCache = Substitute.For<IGameRoundService>();
                _ = fakeGameRoundCache.ExistAsync(roundId).Returns(ValueTask.FromResult(true));
                _ = services.AddSingleton(fakeGameRoundCache);

                var fakeGameRound = Substitute.For<IGameRoundRepository>();

                _ = fakeGameRound.UpdateRoundOpenAsync(new(
                    roundId,
                    data.UpdatedOn,
                    data.BetAmount,
                    data.ValidBetAmount,
                    data.Commission,
                    data.Turnover,
                    data.AdditionalData));

                var gameRound = new GameRoundInfo(
                    RoundId: roundId,
                    ExternalRoundId: externalRoundId,
                    GameProviderId: gameProviderId,
                    PlayerCode: playerCode,
                    GameId: gameId,
                    CurrencyCode: "CNY",
                    Status: GameRoundStatus.Open,
                    GameProviderConfigurationId: Guid.NewGuid(),
                    WalletId: Guid.NewGuid(),
                    CreatedOn: DateTimeOffset.UtcNow,
                    ClosedOn: null,
                    UpdatedOn: DateTimeOffset.UtcNow,
                    BrandId: Guid.NewGuid(),
                    ExternalCreatedOn: DateTimeOffset.UtcNow,
                    ExternalClosedOn: null,
                    BetAmount: data.BetAmount,
                    ValidBetAmount: data.ValidBetAmount,
                    SettleAmount: null,
                    Commission: data.Commission,
                    Turnover: data.Turnover,
                    WinLoss: null,
                    TieBetAdjustment: null,
                    ClientStatus: GameRoundClientStatus.Open,
                    ReopenTurnoverAdjustment: null,
                    ReopenedOn: null,
                    EffectiveBettingPercentage: null,
                    AdditionalData: data.AdditionalData ?? string.Empty);

                _ = fakeGameRound.GetAsync(roundId).Returns(ValueTask.FromResult(gameRound));

                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddSingleton(fakeGameRoundRecord);

                _ = services.AddFakeNatsMessageQueue();
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            await sut.UpdateRoundOpenAsync(roundId, data).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Any<GameRoundInfo>());
        }

        [TestMethod]
        public async Task GameRounds_UpdateRoundReopen()
        {
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var roundId = Guid.NewGuid();

            var data = new UpdateRoundReopenViewModel(
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

            var expected = new UpdateRoundResultViewModel(true);

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            await using var web = CreateTestSettingSite(services =>
            {
                var fakeGameRoundCache = Substitute.For<IGameRoundService>();
                _ = fakeGameRoundCache.ExistAsync(roundId).Returns(ValueTask.FromResult(true));
                _ = services.AddSingleton(fakeGameRoundCache);

                var fakeGameRound = Substitute.For<IGameRoundRepository>();

                _ = fakeGameRound.UpdateRoundReopenAsync(new(
                    roundId,
                    data.Status,
                    data.ClosedOn,
                    data.UpdatedOn,
                    data.ExternalClosedOn,
                    data.BetAmount,
                    data.ValidBetAmount,
                    data.SettleAmount,
                    data.Commission,
                    data.Turnover,
                    data.WinLoss,
                    data.TieBetAdjustment,
                    data.ClientStatus,
                    data.AdditionalData));

                var gameRound = new GameRoundInfo(
                    RoundId: roundId,
                    ExternalRoundId: externalRoundId,
                    GameProviderId: gameProviderId,
                    PlayerCode: playerCode,
                    GameId: gameId,
                    CurrencyCode: "CNY",
                    Status: GameRoundStatus.Closed,
                    GameProviderConfigurationId: Guid.NewGuid(),
                    WalletId: Guid.NewGuid(),
                    CreatedOn: DateTimeOffset.UtcNow,
                    ClosedOn: null,
                    UpdatedOn: DateTimeOffset.UtcNow,
                    BrandId: Guid.NewGuid(),
                    ExternalCreatedOn: DateTimeOffset.UtcNow,
                    ExternalClosedOn: null,
                    BetAmount: data.BetAmount,
                    ValidBetAmount: data.ValidBetAmount,
                    SettleAmount: data.SettleAmount,
                    Commission: data.Commission,
                    Turnover: data.Turnover,
                    WinLoss: data.WinLoss,
                    TieBetAdjustment: null,
                    ClientStatus: GameRoundClientStatus.Open,
                    ReopenTurnoverAdjustment: null,
                    ReopenedOn: null,
                    EffectiveBettingPercentage: null,
                    AdditionalData: data.AdditionalData ?? string.Empty);

                _ = fakeGameRound.GetAsync(roundId).Returns(ValueTask.FromResult(gameRound));

                _ = services.AddSingleton(fakeGameRound);

                _ = services.AddSingleton(fakeGameRoundRecord);

                _ = services.AddFakeNatsMessageQueue();
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            await sut.UpdateRoundReopenAsync(roundId, data).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Any<GameRoundInfo>());
        }

        private static WebApplicationFactory<Program> CreateTestSettingSite(Action<IServiceCollection>? configureServices = null)
            => new GameRoundsApplicationUnitTest(hostBuilder =>
            {
                _ = hostBuilder
                .ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.Sources.Clear();
                })
                .ConfigureServices(services =>
                {
                    configureServices?.Invoke(services);
                    /*
                     var fake = Substitute.For<ISerialNumberGenerator>();
                     services.AddSingleton(fake);
                    */
                });
            });
    }
}
