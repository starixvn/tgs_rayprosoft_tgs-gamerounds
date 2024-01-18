using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

internal class GameRoundsClient : IGameRoundsClient
{
    private static readonly JsonSerializerOptions _SerializerOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _httpClient;

    public GameRoundsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid roundId, CancellationToken cancellationToken = default)
        => await _httpClient.GetAsync($"/gamerounds/{roundId}", cancellationToken)
        .ToAsyncEnumerable()
        .Do(response => response.EnsureSuccessStatusCode())
        .Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
        .SelectMany(response => response.Content
            .ReadFromJsonAsync<GameRoundViewModel>(_SerializerOptions, cancellationToken)
            .ToAsyncEnumerable())
        .FirstOrDefaultAsync(cancellationToken: cancellationToken)
        .ConfigureAwait(false);

    public async ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default)
    {
        var path = new Uri(_httpClient.BaseAddress!, $"gamerounds/game");
        var builder = new UriBuilder(path);
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["gameProviderId"] = gameProviderId.ToString();
        query["externalRoundId"] = externalRoundId;
        query["gameId"] = gameId.ToString();
        query["walletId"] = walletId.ToString();
        builder.Query = query.ToString();
        string url = builder.ToString();

        return await _httpClient.GetAsync(url, cancellationToken)
         .ToAsyncEnumerable()
         .Do(response => response.EnsureSuccessStatusCode())
         .Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
         .SelectMany(response => response.Content
             .ReadFromJsonAsync<GameRoundViewModel>(_SerializerOptions, cancellationToken)
             .ToAsyncEnumerable())
         .FirstOrDefaultAsync(cancellationToken: cancellationToken)
         .ConfigureAwait(false); ;
    }

    public async ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, string externalGameId, Guid walletId, CancellationToken cancellationToken = default)
    {
        var path = new Uri(_httpClient.BaseAddress!, $"gamerounds/external-game");
        var builder = new UriBuilder(path);
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["gameProviderId"] = gameProviderId.ToString();
        query["externalRoundId"] = externalRoundId;
        query["externalGameId"] = externalGameId;
        query["walletId"] = walletId.ToString();
        builder.Query = query.ToString();
        string url = builder.ToString();

        return await _httpClient.GetAsync(url, cancellationToken)
         .ToAsyncEnumerable()
         .Do(response => response.EnsureSuccessStatusCode())
         .Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
         .SelectMany(response => response.Content
             .ReadFromJsonAsync<GameRoundViewModel>(_SerializerOptions, cancellationToken)
             .ToAsyncEnumerable())
         .FirstOrDefaultAsync(cancellationToken: cancellationToken)
         .ConfigureAwait(false); ;
    }

    public async ValueTask<GameRoundDetailViewModel?> GetRoundDetailAsync(Guid roundId, CancellationToken cancellationToken = default)
            => await _httpClient.GetAsync($"/gamerounds/{roundId}/getdetail", cancellationToken)
        .ToAsyncEnumerable()
        .Do(response => response.EnsureSuccessStatusCode())
        .Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
        .SelectMany(response => response.Content
            .ReadFromJsonAsync<GameRoundDetailViewModel>(_SerializerOptions, cancellationToken)
            .ToAsyncEnumerable())
        .FirstOrDefaultAsync(cancellationToken: cancellationToken)
        .ConfigureAwait(false);
}
