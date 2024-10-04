using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Entities;
using Tgs.GameRounds.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameRoundsRepositories(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.AddDbContext<TgsContext>(dbContextOptions)
		.AddTransient<IRoundRepository, RoundRepository>()
		.Decorate<IRoundRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<RoundCacheLockedRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(3)))
		.Decorate<IRoundRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<RoundCacheGetRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(4.5)));
}
