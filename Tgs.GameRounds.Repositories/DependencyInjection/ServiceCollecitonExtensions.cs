using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StackExchange.Redis;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories;
using Tgs.GameRounds.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameRoundBetRepositories(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> configure,
        Func<IServiceProvider, IDatabase> redisDatabaseResolver,
        Func<IServiceProvider, IMongoDatabase> mongoDatabaseResolver)
        => services
            .AddDbContext<BetContext>(configure)
            .AddTransient<IGameRoundRepository, GameRoundRepository>()
            .Decorate<IGameRoundRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameRoundCacheRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(30)))
            .AddTransient<IGameRoundRecord>(sp => ActivatorUtilities.CreateInstance<GameRoundRecordRepository>(sp, mongoDatabaseResolver(sp)))
            .AddTransient<IFailedGameRound, FailedGameRoundRepository>();

    public static IServiceCollection AddGameRoundRepositories(
        this IServiceCollection services,
        Func<IServiceProvider, IDatabase> redisDatabaseResolver)
        => services
            .AddTransient<IGameRoundLocked>((sp) => ActivatorUtilities.CreateInstance<GameRoundLocked>(sp, redisDatabaseResolver(sp)));

    public static IServiceCollection AddGameRoundTgsRepositories(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> configure)
        => services
        .AddDbContext<TgsContext>(configure)
        .AddTransient<IGameProviderRepository, GameProviderRepository>()
        .Decorate<IGameProviderRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameProviderCacheRepository>(sp, innerRepository))
        .AddTransient<IGameRepository, GameRepository>()
        .Decorate<IGameRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameCacheRepository>(sp, innerRepository));
}
