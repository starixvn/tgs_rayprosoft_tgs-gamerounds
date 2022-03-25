using Microsoft.Extensions.DependencyInjection;

namespace Tgs.GameRounds.Core.Configuration
{
    public class GameRoundConfiguration
    {
        public IServiceCollection Services { get; }

        internal Type SourceStoreGameProviderType { get; private set; } = typeof(NoopGameProvider);

        internal Type SourceStoreGameType { get; private set; } = typeof(NoopGame);

        public GameRoundConfiguration(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public GameRoundConfiguration SetGameProviders<TGameProvider>(Func<IServiceProvider, object[]>? parametersFactory = null)
            where TGameProvider : class, IGameProvider
        {
            var sourceType = SourceStoreGameProviderType;

            _ = Services.AddTransient(sp =>
            {
                var parameters = new List<object>
                  {
                      sp.GetRequiredService(sourceType)
                  };
                if (parametersFactory is not null)
                    parameters.AddRange(parametersFactory(sp));

                return ActivatorUtilities.CreateInstance<TGameProvider>(
                    sp,
                    parameters.ToArray());
            });

            SourceStoreGameProviderType = typeof(TGameProvider);

            return this;
        }

        public GameRoundConfiguration SetGames<TGame>(Func<IServiceProvider, object[]>? parametersFactory = null)
            where TGame : class, IGame
        {
            var sourceType = SourceStoreGameType;

            _ = Services.AddTransient(sp =>
            {
                var parameters = new List<object>
                  {
                      sp.GetRequiredService(sourceType)
                  };
                if (parametersFactory is not null)
                    parameters.AddRange(parametersFactory(sp));

                return ActivatorUtilities.CreateInstance<TGame>(
                    sp,
                    parameters.ToArray());
            });

            SourceStoreGameType = typeof(TGame);

            return this;
        }
    }
}
