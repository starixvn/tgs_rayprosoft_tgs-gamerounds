using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.Repositories.UnitTest;

[TestClass]
public class GameRoundCacheRepositoryUnitTest
{
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
}
