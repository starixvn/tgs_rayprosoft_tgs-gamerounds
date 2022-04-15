using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StackExchange.Redis;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Configuration;
using Tgs.GameRounds.Repositories;
using Tgs.GameRounds.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static GameRoundConfiguration AddGameRoundBetRepositories(
            this GameRoundConfiguration configuration,
            Action<IServiceProvider, DbContextOptionsBuilder> configure,
            Func<IServiceProvider, IDatabase> redisDatabaseResolver)
        {
            _ = configuration.Services.AddSerialNumberGenerator();

            _ = configuration.Services.AddDbContext<BetContext>(configure);

            return configuration
                .SetGameRounds<GameRoundRepository>()
                .SetGameRounds<GameRoundRedisRepository>(sp => new object[] { redisDatabaseResolver(sp), TimeSpan.FromMinutes(30) });
        }

        public static IServiceCollection AddGameRoundRepositories(
                    this IServiceCollection services,
            Func<IServiceProvider, IMongoDatabase> mongoDatabaseResolver)
            => services
                .AddSerialNumberGenerator()
                .AddTransient<IGameRoundRecordRepository>(sp => ActivatorUtilities.CreateInstance<GameRoundRecordRepository>(sp, mongoDatabaseResolver(sp)));

        public static GameRoundConfiguration AddGameRoundTgsRepositories(
            this GameRoundConfiguration configuration,
             Action<IServiceProvider, DbContextOptionsBuilder> configure,
             Func<IServiceProvider, IDatabase> redisDatabaseResolver)
        {
            _ = configuration.Services.AddSerialNumberGenerator();

            _ = configuration.Services.AddDbContext<TgsContext>(configure);

            return configuration
                .SetGameProviders<GameProviderRepository>()
                .SetGameProviders<GameProviderRedisRepository>(sp => new object[] { redisDatabaseResolver(sp), TimeSpan.FromHours(1) })
                .SetGames<GameRepository>()
                .SetGames<GameRedisRepository>(sp => new object[] { redisDatabaseResolver(sp), TimeSpan.FromHours(1) });
        }
    }
}
