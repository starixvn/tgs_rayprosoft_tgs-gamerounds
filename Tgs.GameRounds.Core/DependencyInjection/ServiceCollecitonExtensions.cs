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
                .AddTransient<IGameRoundService>(sp => ActivatorUtilities.CreateInstance<GameRoundService>(sp, redisDatabaseResolver(sp), TimeSpan.FromMinutes(30)));
    }
}
