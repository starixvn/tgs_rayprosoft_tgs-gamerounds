using Microsoft.EntityFrameworkCore;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories;
using Tgs.GameRounds.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddRoundRepositories(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> configure)
            => services
                .AddDbContext<BetContext>(configure)
                .AddTransient<IGameRoundRepository, GameRoundRepository>();
    }
}
