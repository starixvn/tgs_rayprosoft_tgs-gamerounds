using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OpenTelemetry.Exporter;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.IntegrationTests;

[TestClass]
public partial class GameRoundsClientUnitTest
{
    [TestMethod]
    public async Task GameRounds_GetGameRound()
    {
        var roundId = Guid.NewGuid();
        var expected = GameRoundStatus.Open;

        var data = new GameRoundResult(
           RoundId: roundId,
           ExternalRoundId: Guid.NewGuid().ToString("N"),
           GameProviderId: Guid.NewGuid(),
           GameProviderCode: "DG",
           PlayerCode: "test",
           GameId: 1,
           ExternalGameId: "Bacc",
           CurrencyCode: "CNY",
           Status: expected,
           GameProviderConfigurationId: Guid.NewGuid(),
           WalletId: Guid.NewGuid(),
           CreatedOn: DateTimeOffset.UtcNow,
           ClosedOn: null,
           UpdatedOn: DateTimeOffset.UtcNow,
           BrandId: Guid.NewGuid(),
           ExternalCreatedOn: null,
           ExternalClosedOn: null,
           BetAmount: 0,
           ValidBetAmount: 0,
           SettleAmount: 0,
           Commission: 0,
           Turnover: 0,
           CompanyWinLoss: 0,
           TieBetAdjustment: 0,
           ClientStatus: GameRoundClientStatus.Open,
           ReopenTurnoverAdjustment: null,
           ReopenedOn: null,
           EffectiveBettingPercentage: null,
           AdditionalData: JsonSerializer.Serialize(new { roundId }));

        var web = CreateTestSettingSite(services =>
        {
            var fakeService = Substitute.For<IGetService<GetGameRoundQuery, GameRoundResult>>();

            _ = fakeService
                .GetAsync(
                    Arg.Any<GetGameRoundQuery>())
                .Returns(ValueTask.FromResult(data));

            _ = services.AddSingleton(fakeService);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.GetGameRoundAsync(roundId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(true, actual.Success);
        Assert.AreEqual(HttpStatusCode.OK, actual.HttpStatusCode);
        Assert.IsNotNull(actual.Data);
    }

    [TestMethod]
    public async Task GameRounds_GetGameRoundByGame()
    {
        var roundId = Guid.NewGuid();
        Guid gameProviderId = Guid.NewGuid();
        string externalRoundId = Guid.NewGuid().ToString("N");
        var externalGameId = "Bacc";
        var gameProviderCode = "DG";
        int gameId = 1;
        Guid walletId = Guid.NewGuid();
        var data = new GameRoundInfo(
          RoundId: roundId,
          ExternalRoundId: externalRoundId,
          GameProviderId: gameProviderId,
          PlayerCode: "test",
          GameId: gameId,
          CurrencyCode: "CNY",
          Status: GameRoundStatus.Open,
          GameProviderConfigurationId: Guid.NewGuid(),
          WalletId: walletId,
          CreatedOn: DateTimeOffset.UtcNow,
          ClosedOn: null,
          UpdatedOn: DateTimeOffset.UtcNow,
          BrandId: Guid.NewGuid(),
          ExternalCreatedOn: null,
          ExternalClosedOn: null,
          BetAmount: 0,
          ValidBetAmount: 0,
          SettleAmount: 0,
          Commission: 0,
          Turnover: 0,
          CompanyWinLoss: 0,
          TieBetAdjustment: 0,
          ClientStatus: GameRoundClientStatus.Open,
          ReopenTurnoverAdjustment: null,
          ReopenedOn: null,
          EffectiveBettingPercentage: null);

        var web = CreateTestSettingSite(services =>
        {
            var fakeRepository = Substitute.For<IGameRoundRepository>();

            _ = fakeRepository.GetAsync(gameProviderId, externalRoundId, gameId, walletId).Returns(ValueTask.FromResult<GameRoundInfo?>(data));

            _ = services.AddSingleton(fakeRepository);

            var fakeGame = Substitute.For<IGameRepository>();

            _ = fakeGame.GetExternalIdAsync(data.GameId).Returns(ValueTask.FromResult(externalGameId));

            _ = services.AddSingleton(fakeGame);

            var fakeGameProvider = Substitute.For<IGameProviderRepository>();

            _ = fakeGameProvider.GetCode(data.GameProviderId).Returns(ValueTask.FromResult(gameProviderCode));

            _ = services.AddSingleton(fakeGameProvider);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.GetGameRoundAsync(gameProviderId, externalRoundId, gameId, walletId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(true, actual.Success);
        Assert.AreEqual(HttpStatusCode.OK, actual.HttpStatusCode);
        Assert.IsNotNull(actual.Data);
        Assert.AreEqual(roundId, actual.Data.RoundId);
        Assert.AreEqual(externalGameId, actual.Data.ExternalGameId);
        Assert.AreEqual(gameProviderCode, actual.Data.GameProviderCode);
    }

    [TestMethod]
    public async Task GameRounds_GetRoundDetail()
    {
        var roundId = Guid.NewGuid();
        var data = new GameRoundDetailInfo(
           RoundId: roundId,
           AdditionalData: JsonSerializer.Serialize(new
           {
               roundId
           }));

        var web = CreateTestSettingSite(services =>
        {
            var fakeGameRound = Substitute.For<IGameRoundRepository>();

            _ = fakeGameRound.GetRoundDetailAsync(roundId).Returns(ValueTask.FromResult<GameRoundDetailInfo?>(data));

            _ = services.AddSingleton(fakeGameRound);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.GetRoundDetailAsync(roundId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(true, actual.Success);
        Assert.AreEqual(HttpStatusCode.OK, actual.HttpStatusCode);
        Assert.IsNotNull(actual.Data);
        Assert.AreEqual(roundId, actual.Data.RoundId);
    }

    private static WebApplicationFactory<Program> CreateTestSettingSite(Action<IServiceCollection>? configureServices = null)
        => new GameRoundsApplicationUnitTest(hostBuilder => _ = hostBuilder
            .ConfigureAppConfiguration((_, configBuilder) => configBuilder.Sources.Clear())
            .ConfigureServices(services =>
            {
                _ = services
                .AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
                    sp => new ConfigureNamedOptions<OtlpExporterOptions>(
                        Options.DefaultName,
                        exporterOptions => exporterOptions.TimeoutMilliseconds = 0));

                configureServices?.Invoke(services);
            }));
}
