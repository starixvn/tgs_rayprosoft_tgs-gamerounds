using System.Reflection;
using StackExchange.Redis;
using Tgs.GameRounds.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddGameRoundsCore(
            this IServiceCollection services,
            Func<IServiceProvider, IDatabase> redisDatabaseResolver)
            => services
                .RegisterAllServiceUtilityTypes(
                    Assembly.GetExecutingAssembly(),
                    ServiceLifetime.Transient)
                .AddTransient<IGameRoundCacheService>(sp => ActivatorUtilities.CreateInstance<GameRoundCacheService>(sp, redisDatabaseResolver(sp), TimeSpan.FromMinutes(30)));
    }
}
