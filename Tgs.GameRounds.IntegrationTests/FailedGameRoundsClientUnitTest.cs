using System;
using System.Text.Json;
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
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.IntegrationTests;

[TestClass]
public class FailedGameRoundsClientUnitTest
{
    [TestMethod]
    public async Task Add_Normal_Process()
    {
        var model = new AddFailedGameRoundViewModel
        (
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "dg",
            ExternalRoundId: Guid.NewGuid().ToString("N"),
            PlayerCode: "test",
            GameId: -1,
            ExternalGameId: "bacc",
            Detail: JsonSerializer.Serialize(new { RoundId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }));

        var web = CreateTestSettingSite(services =>
        {
            var fakeFailedGameRound = Substitute.For<IFailedGameRound>();

            _ = fakeFailedGameRound.AddAsync(Arg.Any<AddFailedGameRound>()).Returns(Task.CompletedTask);

            _ = services.AddSingleton(fakeFailedGameRound);
        });

        var httpClient = web.CreateClient();

        var sut = new FailedGameRoundsClient(httpClient);

        await sut.AddAsync(model).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Get_Normal_Process()
    {
        Guid gameProviderId = Guid.NewGuid();
        string externalRoundId = Guid.NewGuid().ToString("N");
        string playerCode = "test";
        int gameId = -1;

        var web = CreateTestSettingSite(services =>
        {
            var fakeFailedGameRound = Substitute.For<IFailedGameRound>();

            var info = new FailedGameRoundInfo()
            {
                FailedGameRoundId = Guid.NewGuid(),
                GameProviderId = gameProviderId,
                GameProviderCode = "DG",
                ExternalRoundId = externalRoundId,
                PlayerCode = playerCode,
                GameId = gameId,
                ExternalGameId = "bacc",
                Detail = "{}",
                AutoStatus = FailedGameRoundAutoStatus.Manual,
                ProcesStatus = FailedGameRoundProcesStatus.Process,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
            };

            _ = fakeFailedGameRound.GetAsync(
                gameProviderId,
                externalRoundId,
                gameId,
                playerCode,
                Arg.Any<CancellationToken>()
                ).Returns(Task.FromResult<FailedGameRoundInfo?>(info));

            _ = services.AddSingleton(fakeFailedGameRound);
        });

        var httpClient = web.CreateClient();

        var sut = new FailedGameRoundsClient(httpClient);

        var actual = await sut
            .GetAsync(
                gameProviderId,
                externalRoundId,
                gameId,
                playerCode).ConfigureAwait(false);

        Assert.IsNotNull(actual);
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
