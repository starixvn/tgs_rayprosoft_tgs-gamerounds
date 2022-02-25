using Microsoft.EntityFrameworkCore;
using Tgs.DataBase.Round.Core;
using Tgs.DataBase.Round.Repositories;
using Tgs.DataBase.Round.Repositories.Entities;

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
