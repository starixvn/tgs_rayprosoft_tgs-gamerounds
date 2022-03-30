using System.Reflection;
using StackExchange.Redis;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddGameRoundsCore(
            this IServiceCollection services,
            Action<GameRoundConfiguration> configure,
            Func<IServiceProvider, IDatabase> redisDatabaseResolver)
        {
            var configuration = new GameRoundConfiguration(services);

            configure(configuration);
            return services
                      .RegisterAllServiceUtilityTypes(
                          Assembly.GetExecutingAssembly(),
                          ServiceLifetime.Transient)
                      .AddTransient<IGameRoundService>(sp => ActivatorUtilities.CreateInstance<GameRoundService>(sp, redisDatabaseResolver(sp), TimeSpan.FromMinutes(30)))
                      .AddSingleton<NoopGame>()
                      .AddTransient(sp => (IGame)sp.GetRequiredService(configuration.SourceStoreGameType))
                      .AddSingleton<NoopGameProvider>()
                      .AddTransient(sp => (IGameProvider)sp.GetRequiredService(configuration.SourceStoreGameProviderType));
        }
    }
}
