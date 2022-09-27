using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OpenTelemetry.Exporter;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core.HandleServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.IntegrationTests;

[TestClass]
public class GameRoundsWorkClientUnitTest
{
	[TestMethod]
	public async Task GameRounds_GetGameRound()
	{
		var model = new GameRoundsWorkViewModel()
		{
			ExternalRoundId = Guid.NewGuid().ToString("N"),
			GameProviderId = Guid.NewGuid(),
			GameId = 1,
			PlayerCode = "test",
			CurrencyCode = "CNY",
			WalletId = Guid.NewGuid(),
			BrandId = Guid.NewGuid(),
			GameProviderConfigurationId = Guid.NewGuid(),
			Status = GameRoundStatus.Closed,
			ClientStatus = GameRoundClientStatus.Win,
			ExternalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-1),
			ExternalClosedOn = DateTimeOffset.UtcNow,
			BetAmount = 10,
			ValidBetAmount = 10,
			SettleAmount = 20,
			Commission = 0,
			Turnover = 10,
			PlayerWinLoss = 10,
			AdditionalData = "{}"
		};

		var web = CreateTestSettingSite(services =>
		{
			var fakeService = Substitute.For<ICommandService<GameRoundWorkCommand>>();

			_ = fakeService
				.ExecuteAsync(
					Arg.Any<GameRoundWorkCommand>())
				.Returns(ValueTask.CompletedTask);

			_ = services.AddSingleton(fakeService);
		});

		var httpClient = web.CreateClient();

		var sut = new GameRoundsWorkClient(httpClient);

		await sut
			.ExecuteAsync(
				model)
			.ConfigureAwait(false);
	}

	private static WebApplicationFactory<Program> CreateTestSettingSite(Action<IServiceCollection>? configureServices = null)
		=> new GameRoundsApplicationUnitTest(hostBuilder => _ = hostBuilder
			.ConfigureAppConfiguration((_, configBuilder) => configBuilder.Sources.Clear())
			.ConfigureServices(services =>
			{
				_ = services
				.AddFakeNatsMessageQueue()
				.AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
					sp => new ConfigureNamedOptions<OtlpExporterOptions>(
						Options.DefaultName,
						exporterOptions => exporterOptions.TimeoutMilliseconds = 0));

				configureServices?.Invoke(services);
			}));
}
