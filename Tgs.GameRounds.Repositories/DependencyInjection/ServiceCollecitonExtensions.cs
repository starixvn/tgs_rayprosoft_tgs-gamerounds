using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories;
using Tgs.GameRounds.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddGameRoundBetRepositories(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> configure)
            => services
            .AddSerialNumberGenerator()
            .AddDbContext<BetContext>(configure)
            .AddTransient<IGameRoundRepository, GameRoundRepository>();

        public static IServiceCollection AddGameRoundRepositories(
                    this IServiceCollection services,
            Func<IServiceProvider, IMongoDatabase> mongoDatabaseResolver)
            => services
                .AddSerialNumberGenerator()
                .AddTransient<IGameRoundRecordRepository>(sp => ActivatorUtilities.CreateInstance<GameRoundRecordRepository>(sp, mongoDatabaseResolver(sp)));

        public static IServiceCollection AddGameRoundTgsRepositories(
            this IServiceCollection services,
             Action<IServiceProvider, DbContextOptionsBuilder> configure)
            => services
            .AddDbContext<TgsContext>(configure)
            .AddTransient<IGameRepository, GameRepository>();
    }
}
