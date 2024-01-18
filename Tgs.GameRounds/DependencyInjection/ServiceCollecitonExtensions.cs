using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameRounds(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> tgsConfigure,
        Action<IServiceProvider, DbContextOptionsBuilder> betConfigure)
        => services
        .AddGameRoundBetRepositories(betConfigure)
        .AddGameRoundTgsRepositories(tgsConfigure);
}
