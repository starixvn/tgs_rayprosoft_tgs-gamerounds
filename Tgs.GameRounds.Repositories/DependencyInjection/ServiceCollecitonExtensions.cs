using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StackExchange.Redis;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories;
using Tgs.GameRounds.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameRoundBetRepositories(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> configure,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver,
		Func<IServiceProvider, IMongoDatabase> mongoDatabaseResolver)
		=> services
			.AddDbContext<BetContext>(configure)
			.AddTransient<IGameRound, GameRoundRepository>()
			.Decorate<IGameRound>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameRoundCacheRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(30)))
			.AddTransient<IGameRoundRecord>(sp => ActivatorUtilities.CreateInstance<GameRoundRecordRepository>(sp, mongoDatabaseResolver(sp)));

	public static IServiceCollection AddGameRoundRepositories(
		this IServiceCollection services,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
			.AddTransient<IGameRoundLocked>((sp) => ActivatorUtilities.CreateInstance<GameRoundLocked>(sp, redisDatabaseResolver(sp)));

	public static IServiceCollection AddGameRoundTgsRepositories(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> configure,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
			.AddDbContext<TgsContext>(configure)
			.AddTransient<IGameProvider, GameProviderRepository>()
			.Decorate<IGameProvider>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameProviderCacheRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromHours(1)))
			.AddTransient<IGame, GameRepository>()
			.Decorate<IGame>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameCacheRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromHours(1)));
}
