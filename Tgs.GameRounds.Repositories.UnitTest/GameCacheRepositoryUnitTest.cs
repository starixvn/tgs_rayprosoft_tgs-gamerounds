using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.Repositories.UnitTest;

[TestClass]
public class GameCacheRepositoryUnitTest
{
    [TestMethod]
    public async Task Get_ExternalId()
    {
        var fakeRepository = Substitute.For<IGameRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var id = 1;
        var key = string.Format("gamerounds:game:get:externalid:{0}", id);

        _ = fakeMemoryCache
            .TryGetValue(
                key: Arg.Is(key),
                value: out _)
             .Returns(false);

        var externalId = "baccarat";
        _ = fakeRepository
            .GetExternalIdAsync(
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(externalId);

        var repository = new GameCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        var actual = await repository
            .GetExternalIdAsync(
                id: id)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(externalId, actual);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                externalId);
    }

    [TestMethod]
    public async Task Get_Id()
    {
        var fakeRepository = Substitute.For<IGameRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var gameProviderId = Guid.NewGuid();
        var externalId = "baccarat";
        var key = string.Format("gamerounds:game:get:id:{0}:{1}", gameProviderId, externalId);

        _ = fakeMemoryCache
            .TryGetValue(
                key: Arg.Is(key),
                value: out _)
             .Returns(false);

        var id = 1;
        _ = fakeRepository
            .GetIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                externalId: Arg.Is(externalId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(id);

        var repository = new GameCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        var actual = await repository
            .GetIdAsync(
                gameProviderId: gameProviderId,
                externalId: externalId)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(id, actual);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                externalId);
    }
}
