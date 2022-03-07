using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories;
using Tgs.GameRounds.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddGameRoundRepositories(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> configure,
            Func<IServiceProvider, IMongoDatabase> mongoDatabaseResolver)
            => services
                .AddSerialNumberGenerator()
                .AddDbContext<BetContext>(configure)
                .AddTransient<IGameRoundRecordRepository>(sp => ActivatorUtilities.CreateInstance<GameRoundRecordRepository>(sp, mongoDatabaseResolver(sp)))
                .AddTransient<IGameRoundRepository, GameRoundRepository>();
    }
}
