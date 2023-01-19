using Microsoft.EntityFrameworkCore;
using Tgs.GameRounds;
using Tgs.GameRounds.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameRounds(
        this IServiceCollection services,
        Action<MongoDBOptions, IServiceProvider> mongoDBconfigure,
        Action<RedisOptions, IServiceProvider> redisDBconfigure,
        Action<IServiceProvider, DbContextOptionsBuilder> tgsConfigure,
        Action<IServiceProvider, DbContextOptionsBuilder> betConfigure,
        Action<GameLockedOptions, IServiceProvider> gameLockedOptions)
        => services
            .AddOptions<MongoDBOptions>()
            .Configure(mongoDBconfigure)
            .Services
            .AddOptions<RedisOptions>()
            .Configure(redisDBconfigure)
            .Services
            .AddSingleton<MongoDBConnectionManager>()
            .AddSingleton<RedisConnectionManager>()
            .AddGameRoundsCore(gameLockedOptions)
            .AddMemoryCache()
            .AddGameRoundBetRepositories(
                betConfigure,
                (sp) => sp.GetRequiredService<RedisConnectionManager>().Database,
                (sp) => sp.GetRequiredService<MongoDBConnectionManager>().Database)
            .AddGameRoundTgsRepositories(
                tgsConfigure,
                (sp) => sp.GetRequiredService<RedisConnectionManager>().Database)
            .AddGameRoundRepositories(
                (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
