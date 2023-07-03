using Microsoft.EntityFrameworkCore;
using Tgs.GameRounds;
using Tgs.GameRounds.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameRounds(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> redisDBconfigure,
        Action<IServiceProvider, DbContextOptionsBuilder> tgsConfigure,
        Action<IServiceProvider, DbContextOptionsBuilder> betConfigure,
        Action<GameLockedOptions, IServiceProvider> gameLockedOptions)
        => services
            .AddOptions<RedisOptions>()
            .Configure(redisDBconfigure)
            .Services
            .AddSingleton<RedisConnectionManager>()
            .AddGameRoundsCore(gameLockedOptions)
            .AddMemoryCache()
            .AddGameRoundBetRepositories(
                betConfigure)
            .AddGameRoundTgsRepositories(
                tgsConfigure)
            .AddGameRoundLockedRepositories(
                (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
