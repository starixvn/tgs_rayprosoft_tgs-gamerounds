using Microsoft.Extensions.Options;
using Tgs.GameRounds.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRoundsClient(
        this IServiceCollection services,
        Action<GameRoundsClientOptions, IServiceProvider> configure)
        => services
            .AddOptions<GameRoundsClientOptions>()
            .Configure(configure)
            .Services
            .AddHttpClient<IGameRoundsClient, GameRoundsClient>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<GameRoundsClientOptions>>().Value;
                httpClient.BaseAddress = options.BaseUrl;
            })
            .Services;
}
