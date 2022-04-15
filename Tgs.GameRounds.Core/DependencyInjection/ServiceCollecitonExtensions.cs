using System.Reflection;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddGameRoundsCore(
            this IServiceCollection services,
            Action<GameRoundConfiguration> configure)
        {
            var configuration = new GameRoundConfiguration(services);

            configure(configuration);
            return services
                      .RegisterAllServiceUtilityTypes(
                          Assembly.GetExecutingAssembly(),
                          ServiceLifetime.Transient)
                      .AddSingleton<NoopGameRound>()
                      .AddTransient(sp => (IGameRound)sp.GetRequiredService(configuration.SourceStoreGameRoundType))
                      .AddSingleton<NoopGameProvider>()
                      .AddTransient(sp => (IGameProvider)sp.GetRequiredService(configuration.SourceStoreGameProviderType))
                      .AddSingleton<NoopGame>()
                      .AddTransient(sp => (IGame)sp.GetRequiredService(configuration.SourceStoreGameType));
        }
    }
}
