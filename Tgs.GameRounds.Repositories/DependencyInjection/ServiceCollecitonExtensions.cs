using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories;
using Tgs.GameRounds.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameRoundBetRepositories(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> configure)
        => services
        .AddDbContext<BetContext>(configure)
        .AddTransient<IGameRoundRepository, GameRoundRepository>();

    public static IServiceCollection AddGameRoundLockedRepositories(
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
        .AddTransient<IGameRepository, GameRepository>();
}
