using Microsoft.EntityFrameworkCore;
using Tgs.DataBase.Rounds.Core;
using Tgs.DataBase.Rounds.Repositories;
using Tgs.DataBase.Rounds.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddRoundRepositories(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> configure)
            => services
                .AddDbContext<BetContext>(configure)
                .AddTransient<IRoundRepository, RoundRepository>();
    }
}
