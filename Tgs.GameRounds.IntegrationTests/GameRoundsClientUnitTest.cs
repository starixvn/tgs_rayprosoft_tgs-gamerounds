using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using OpenTelemetry.Exporter;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.IntegrationTests;

public partial class GameRoundsClientUnitTest
{
    [Fact]
    public async Task GameRounds_Get_GameRound()
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
            var fakeService = Substitute.For<IGetService<GameRoundQuery, GameRoundResult?>>();

            _ = fakeService
                .GetAsync(
                query: Arg.Any<GameRoundQuery>(),
                cancellationToken: Arg.Any<CancellationToken>())
                .Returns(data);

            _ = services.AddSingleton(fakeService);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.GetGameRoundAsync(roundId);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GameRounds_Get_GameRound_ByExternalGame()
    {
        var roundId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        var externalRoundId = Guid.NewGuid().ToString("N");
        var gameProviderCode = "DG";
        var externalGameId = "baccarat";
        var walletId = Guid.NewGuid();

        var data = new GameRoundResult(
           RoundId: roundId,
           ExternalRoundId: Guid.NewGuid().ToString("N"),
           GameProviderId: Guid.NewGuid(),
           GameProviderCode: gameProviderCode,
           PlayerCode: "test",
           GameId: 1,
           ExternalGameId: externalGameId,
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
           EffectiveBettingPercentage: null,
           AdditionalData: JsonSerializer.Serialize(new { roundId }));

        var web = CreateTestSettingSite(services =>
        {
            var fakeService = Substitute.For<IGetService<GameRoundByExternalGameQuery, GameRoundResult?>>();

            var query = new GameRoundByExternalGameQuery(
                GameProviderId: gameProviderId,
                ExternalRoundId: externalRoundId,
                ExternalGameId: externalGameId,
                WalletId: walletId);
            _ = fakeService
                .GetAsync(
                query: Arg.Is(query),
                cancellationToken: Arg.Any<CancellationToken>())
                .Returns(data);

            _ = services.AddSingleton(fakeService);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.GetGameRoundAsync(gameProviderId, externalRoundId, externalGameId, walletId);

        Assert.NotNull(actual);
        Assert.Equal(roundId, actual.RoundId);
        Assert.Equal(externalGameId, actual.ExternalGameId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
    }

    [Fact]
    public async Task GameRounds_Get_GameRound_ByExternalGame_Is_Null()
    {
        var roundId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        var externalRoundId = Guid.NewGuid().ToString("N");
        var externalGameId = "baccarat";
        var walletId = Guid.NewGuid();

        var web = CreateTestSettingSite(services =>
        {
            var fakeService = Substitute.For<IGetService<GameRoundByExternalGameQuery, GameRoundResult?>>();

            var query = new GameRoundByExternalGameQuery(
                GameProviderId: gameProviderId,
                ExternalRoundId: externalRoundId,
                ExternalGameId: externalGameId,
                WalletId: walletId);
            _ = fakeService
                .GetAsync(
                query: Arg.Is(query),
                cancellationToken: Arg.Any<CancellationToken>())
                .Returns((GameRoundResult?)null);

            _ = services.AddSingleton(fakeService);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.GetGameRoundAsync(gameProviderId, externalRoundId, externalGameId, walletId);

        Assert.Null(actual);
    }

    [Fact]
    public async Task GameRounds_Get_GameRound_ByGame()
    {
        var roundId = Guid.NewGuid();
        Guid gameProviderId = Guid.NewGuid();
        string externalRoundId = Guid.NewGuid().ToString("N");
        var externalGameId = "Bacc";
        var gameProviderCode = "DG";
        int gameId = 1;
        Guid walletId = Guid.NewGuid();

        var data = new GameRoundResult(
           RoundId: roundId,
           ExternalRoundId: Guid.NewGuid().ToString("N"),
           GameProviderId: Guid.NewGuid(),
           GameProviderCode: gameProviderCode,
           PlayerCode: "test",
           GameId: gameId,
           ExternalGameId: externalGameId,
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
           EffectiveBettingPercentage: null,
           AdditionalData: JsonSerializer.Serialize(new { roundId }));

        var web = CreateTestSettingSite(services =>
        {
            var fakeService = Substitute.For<IGetService<GameRoundByGameQuery, GameRoundResult?>>();

            var query = new GameRoundByGameQuery(
                GameProviderId: gameProviderId,
                ExternalRoundId: externalRoundId,
                GameId: gameId,
                WalletId: walletId);
            _ = fakeService
                .GetAsync(
                query: Arg.Is(query),
                cancellationToken: Arg.Any<CancellationToken>())
                .Returns(data);

            _ = services.AddSingleton(fakeService);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.GetGameRoundAsync(gameProviderId, externalRoundId, gameId, walletId);

        Assert.NotNull(actual);
        Assert.Equal(roundId, actual.RoundId);
        Assert.Equal(externalGameId, actual.ExternalGameId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
    }

    [Fact]
    public async Task GameRounds_Get_GameRound_ByGame_Is_Null()
    {
        var roundId = Guid.NewGuid();
        Guid gameProviderId = Guid.NewGuid();
        string externalRoundId = Guid.NewGuid().ToString("N");
        int gameId = 1;
        Guid walletId = Guid.NewGuid();

        var web = CreateTestSettingSite(services =>
        {
            var fakeService = Substitute.For<IGetService<GameRoundByGameQuery, GameRoundResult?>>();

            var query = new GameRoundByGameQuery(
                GameProviderId: gameProviderId,
                ExternalRoundId: externalRoundId,
                GameId: gameId,
                WalletId: walletId);
            _ = fakeService
                .GetAsync(
                query: Arg.Is(query),
                cancellationToken: Arg.Any<CancellationToken>())
                .Returns((GameRoundResult?)null);

            _ = services.AddSingleton(fakeService);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.GetGameRoundAsync(gameProviderId, externalRoundId, gameId, walletId);

        Assert.Null(actual);
    }

    [Fact]
    public async Task GameRounds_Get_RoundDetail()
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

            _ = fakeGameRound
            .GetRoundDetailAsync(
                id: Arg.Is(roundId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<GameRoundDetailInfo?>(data));

            _ = services.AddSingleton(fakeGameRound);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.GetRoundDetailAsync(roundId);

        Assert.NotNull(actual);
        Assert.Equal(roundId, actual.RoundId);
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
