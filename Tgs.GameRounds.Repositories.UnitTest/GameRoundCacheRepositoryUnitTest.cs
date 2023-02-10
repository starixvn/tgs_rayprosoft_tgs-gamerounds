using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Repositories.UnitTest;

[TestClass]
public class GameRoundCacheRepositoryUnitTest
{
    [TestMethod]
    public async Task Add_Company_Round()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
        var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
        var gameId = 500001;
        var playerCode = "4ues8xikza7o";
        var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

        var add = new AddCompanyRound(
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

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        _ = await repository
            .AddCompanyRoundAsync(
                source: add)
            .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .AddCompanyRoundAsync(
                source: Arg.Any<AddCompanyRound>(),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Add_Round()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
        var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
        var gameId = 500001;
        var playerCode = "4ues8xikza7o";
        var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

        var add = new AddRound(
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            PlayerCode: playerCode,
            GameId: gameId,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Open,
            GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
            WalletId: walletId,
            BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
            ExternalCreatedOn: DateTimeOffset.UtcNow,
            ExternalClosedOn: null,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 0,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: 0,
            ClientStatus: GameRoundClientStatus.Open,
            AdditionalData: "{}");

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        _ = await repository
            .AddRoundAsync(
                source: add)
            .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .AddRoundAsync(
                source: Arg.Any<AddRound>(),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Adjust_Closed_Round()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var roundId = Guid.NewGuid();

        var adjust = new AdjustClosedRound(
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 0,
            Commission: 0,
            Turnover: 10m,
            CompanyWinLoss: 10m,
            ClientStatus: GameRoundClientStatus.Loss,
            ExternalClosedOn: DateTimeOffset.UtcNow,
            OperatorId: Guid.NewGuid(),
            Remark: "test");

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        await repository
           .AdjustClosedRoundAsync(
           id: roundId,
           source: adjust)
           .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .AdjustClosedRoundAsync(
                id: Arg.Is(roundId),
                source: Arg.Any<AdjustClosedRound>(),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Exist()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var id = Guid.NewGuid();
        var key = string.Format("gamerounds:gameround:exist:{0}", id);

        _ = fakeMemoryCache
            .TryGetValue(
                key: Arg.Is(key),
                value: out _)
             .Returns(false);

        var exist = true;
        _ = fakeRepository
            .ExistAsync(
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(exist);

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        var actual = await repository
            .ExistAsync(
                id: id)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(exist, actual);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                exist);
    }

    [TestMethod]
    public async Task Get_Round()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var id = Guid.NewGuid();

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        _ = await repository
           .GetAsync(
                id: id)
           .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .GetAsync(
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Get_Round_Detail()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var id = Guid.NewGuid();

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        _ = await repository
            .GetRoundDetailAsync(
                 id: id)
            .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .GetRoundDetailAsync(
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Get_Round_External()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var gameProviderId = Guid.NewGuid();
        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameId = 1;
        var walletId = Guid.NewGuid();

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        _ = await repository
            .GetAsync(
                 gameProviderId: gameProviderId,
                 externalRoundId: externalRoundId,
                 gameId: gameId,
                 walletId: walletId)
            .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .GetAsync(
                gameProviderId: Arg.Is(gameProviderId),
                externalRoundId: Arg.Is(externalRoundId),
                gameId: Arg.Is(gameId),
                walletId: Arg.Is(walletId),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Get_Round_External_ExternalCreatedOn()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var gameProviderId = Guid.NewGuid();
        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameId = 1;
        var walletId = Guid.NewGuid();
        var externalCreatedOn = DateTimeOffset.Now;

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        _ = await repository
            .GetAsync(
                 gameProviderId: gameProviderId,
                 externalRoundId: externalRoundId,
                 gameId: gameId,
                 walletId: walletId,
                 externalCreatedOn: externalCreatedOn)
            .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .GetAsync(
                gameProviderId: Arg.Is(gameProviderId),
                externalRoundId: Arg.Is(externalRoundId),
                gameId: Arg.Is(gameId),
                walletId: Arg.Is(walletId),
                externalCreatedOn: Arg.Is(externalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Get_Round_ExternalCreatedOn()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var id = Guid.NewGuid();
        var externalCreatedOn = DateTimeOffset.Now;

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        _ = await repository
           .GetAsync(
                id: id,
                externalCreatedOn: externalCreatedOn)
           .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .GetAsync(
                id: Arg.Is(id),
                externalCreatedOn: Arg.Is(externalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Get_RoundId()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var gameProviderId = Guid.NewGuid();
        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameId = 1;
        var walletId = Guid.NewGuid();

        var key = string.Format("gamerounds:gameround:get:roundid:{0}:{1}:{2}:{3}", gameProviderId, externalRoundId, gameId, walletId);

        _ = fakeMemoryCache
            .TryGetValue(
                key: Arg.Is(key),
                value: out _)
             .Returns(false);

        var roundId = Guid.NewGuid();
        _ = fakeRepository
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                externalRoundId: Arg.Is(externalRoundId),
                gameId: Arg.Is(gameId),
                walletId: Arg.Is(walletId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(roundId);

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        var actual = await repository
            .GetRoundIdAsync(
                gameProviderId: gameProviderId,
                externalRoundId: externalRoundId,
                gameId: gameId,
                walletId: walletId)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(roundId, actual);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                roundId);
    }

    [TestMethod]
    public async Task Get_RoundId_ExternalCreatedOn()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var gameProviderId = Guid.NewGuid();
        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameId = 1;
        var walletId = Guid.NewGuid();
        var externalCreatedOn = DateTimeOffset.Now;

        var key = string.Format("gamerounds:gameround:get:roundid:{0}:{1}:{2}:{3}:{4}", gameProviderId, externalRoundId, gameId, walletId, externalCreatedOn.ToUnixTimeMilliseconds());

        _ = fakeMemoryCache
            .TryGetValue(
                key: Arg.Is(key),
                value: out _)
             .Returns(false);

        var roundId = Guid.NewGuid();
        _ = fakeRepository
            .GetRoundIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                externalRoundId: Arg.Is(externalRoundId),
                gameId: Arg.Is(gameId),
                walletId: Arg.Is(walletId),
                externalCreatedOn: Arg.Is(externalCreatedOn),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(roundId);

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        var actual = await repository
            .GetRoundIdAsync(
                gameProviderId: gameProviderId,
                externalRoundId: externalRoundId,
                gameId: gameId,
                walletId: walletId,
                externalCreatedOn: externalCreatedOn)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(roundId, actual);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                roundId);
    }

    [TestMethod]
    public async Task Get_Status()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var id = Guid.NewGuid();

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        _ = await repository
            .GetStatusAsync(
                 id: id)
            .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .GetStatusAsync(
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Manual_Close_Round()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var roundId = Guid.NewGuid();

        var manual = new ManualCloseRound(
            ExternalClosedOn: DateTimeOffset.UtcNow,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 0,
            Commission: 0,
            Turnover: 10m,
            CompanyWinLoss: 10m,
            ClientStatus: GameRoundClientStatus.Loss,
            OperatorId: Guid.NewGuid(),
            Remark: "test");

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        await repository
           .ManualCloseRoundAsync(
           id: roundId,
           source: manual)
           .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .ManualCloseRoundAsync(
                id: Arg.Is(roundId),
                source: Arg.Any<ManualCloseRound>(),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Update_Round_Closed()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var roundId = Guid.NewGuid();
        var update = new UpdateRoundClosed(
            ExternalClosedOn: DateTimeOffset.UtcNow,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 0,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: -10m,
            ClientStatus: GameRoundClientStatus.Loss,
            AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        await repository
           .UpdateRoundClosedAsync(
           id: roundId,
           source: update)
           .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .UpdateRoundClosedAsync(
                id: Arg.Is(roundId),
                source: Arg.Any<UpdateRoundClosed>(),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Update_Round_Detail()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var roundId = Guid.NewGuid();
        var additionalData = JsonSerializer.Serialize(new { RoundId = roundId });

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        await repository
           .UpdateRoundDetailAsync(
           id: roundId,
           additionalData: additionalData)
           .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .UpdateRoundDetailAsync(
                id: Arg.Is(roundId),
                additionalData: Arg.Is(additionalData),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Update_Round_Open()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var roundId = Guid.NewGuid();
        var update = new UpdateRoundOpen(
            BetAmount: 20m,
            ValidBetAmount: 20m,
            Commission: 0,
            Turnover: 20m);

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        await repository
           .UpdateRoundOpenAsync(
           id: roundId,
           source: update)
           .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .UpdateRoundOpenAsync(
                id: Arg.Is(roundId),
                source: Arg.Is(update),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Update_Round_Reopen()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var roundId = Guid.NewGuid();
        var update = new UpdateRoundReopen(
            Status: GameRoundStatus.Closed,
            ExternalClosedOn: DateTimeOffset.UtcNow,
            BetAmount: 10m,
            ValidBetAmount: 10m,
            SettleAmount: 20m,
            Commission: 0,
            Turnover: 10m,
            PlayerWinLoss: 10m,
            ClientStatus: GameRoundClientStatus.Win,
            AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

        var repository = new GameRoundCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        await repository
           .UpdateRoundReopenAsync(
           id: roundId,
           source: update)
           .ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .UpdateRoundReopenAsync(
                id: Arg.Is(roundId),
                source: Arg.Is(update),
                cancellationToken: Arg.Any<CancellationToken>());
    }
}
