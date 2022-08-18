using System.Reflection;
using Tgs.GameRounds.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameRoundsCore(
		this IServiceCollection services,
		Action<GameLockedOptions, IServiceProvider> gameLockedOptions)
		=> services
			.RegisterAllServiceUtilityTypes(
				Assembly.GetExecutingAssembly(),
				ServiceLifetime.Transient)
			.AddOptions<GameLockedOptions>()
			.Configure(gameLockedOptions)
			.Services
			.AddSerialNumberGenerator()
			.AddTransient<IGameRoundRecordMessage, GameRoundRecordMessage>();
}
