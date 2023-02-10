using System;
using System.Threading;
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
public class GameRoundsCheckingClientUnitTest
{
    [TestMethod]
    public async Task Change()
    {
        var model = new GameRoundCheckingViewModel
        {
            ExternalRoundId = Guid.NewGuid().ToString("N"),
            GameProviderId = Guid.NewGuid(),
            GameId = 1,
            PlayerCode = "test",
            WalletId = Guid.NewGuid(),
            Status = GameRoundStatus.Closed,
            ClientStatus = GameRoundClientStatus.Win,
            ExternalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-1),
            BetAmount = 10,
            ValidBetAmount = 10,
            SettleAmount = 20,
            Commission = 0,
            Turnover = 10,
            PlayerWinLoss = 10
        };

        var web = CreateTestSettingSite(services =>
        {
            var fakeService = Substitute.For<IGetService<GameRoundCheckingQuery, GameRoundCheckingStatus>>();

            _ = fakeService
                .GetAsync(
                    Arg.Any<GameRoundCheckingQuery>(),
                    Arg.Any<CancellationToken>())
                .Returns(GameRoundCheckingStatus.Change);

            _ = services.AddSingleton(fakeService);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsCheckingClient(httpClient);

        var actual = await sut
             .GetCheckingStatusAsync(
                 model)
             .ConfigureAwait(false);

        Assert.AreEqual(GameRoundCheckingStatus.Change, actual);
    }

    [TestMethod]
    public async Task Unchanged()
    {
        var model = new GameRoundCheckingViewModel
        {
            ExternalRoundId = Guid.NewGuid().ToString("N"),
            GameProviderId = Guid.NewGuid(),
            GameId = 1,
            PlayerCode = "test",
            WalletId = Guid.NewGuid(),
            Status = GameRoundStatus.Closed,
            ClientStatus = GameRoundClientStatus.Win,
            ExternalCreatedOn = DateTimeOffset.UtcNow.AddMinutes(-1),
            BetAmount = 10,
            ValidBetAmount = 10,
            SettleAmount = 20,
            Commission = 0,
            Turnover = 10,
            PlayerWinLoss = 10
        };

        var web = CreateTestSettingSite(services =>
        {
            var fakeService = Substitute.For<IGetService<GameRoundCheckingQuery, GameRoundCheckingStatus>>();

            _ = fakeService
                .GetAsync(
                    Arg.Any<GameRoundCheckingQuery>(),
                    Arg.Any<CancellationToken>())
                .Returns(GameRoundCheckingStatus.Unchanged);

            _ = services.AddSingleton(fakeService);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsCheckingClient(httpClient);

        var actual = await sut
             .GetCheckingStatusAsync(
                 model)
             .ConfigureAwait(false);

        Assert.AreEqual(GameRoundCheckingStatus.Unchanged, actual);
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
