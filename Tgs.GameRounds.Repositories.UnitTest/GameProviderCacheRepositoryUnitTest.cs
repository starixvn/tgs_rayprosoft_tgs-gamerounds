using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.Repositories.UnitTest;

[TestClass]
public class GameProviderCacheRepositoryUnitTest
{
    [TestMethod]
    public async Task Get_Code()
    {
        var fakeRepository = Substitute.For<IGameProviderRepository>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();

        var id = Guid.NewGuid();
        var key = string.Format("gamerounds:gameprovider:get:code:{0}", id);

        _ = fakeMemoryCache
            .TryGetValue(
                key: Arg.Is(key),
                value: out _)
             .Returns(false);

        var code = "DG";
        _ = fakeRepository
            .GetCode(
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(code);

        var repository = new GameProviderCacheRepository(
            fakeRepository,
            fakeMemoryCache);

        var actual = await repository
            .GetCode(
                id: id)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(code, actual);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                code);
    }
}
