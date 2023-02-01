using Tgs.GameRounds.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class GameRoundsRegisterExtensions
{
    public static async Task<WebApplication> UseGameRoundsRegister(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameProviderRepository = scope.ServiceProvider.GetRequiredService<IGameProviderRepository>();

            var gameProviderIds = await gameProviderRepository
                .GetIds()
                .ToArrayAsync()
                .ConfigureAwait(false);

            foreach (var gameProviderId in gameProviderIds)
            {
                _ = await gameProviderRepository
                    .GetCode(
                        gameProviderId)
                    .ConfigureAwait(false);
            }
        }

        return app;
    }
}
