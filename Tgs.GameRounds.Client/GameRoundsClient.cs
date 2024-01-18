using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

internal class GameRoundsClient : IGameRoundsClient
{
    private static readonly JsonSerializerOptions _SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
            {
                new JsonStringEnumConverter()
            }
    };

    private readonly HttpClient _httpClient;

    public GameRoundsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid roundId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
            .GetAsync(
                requestUri: $"gamerounds/{roundId}",
                cancellationToken: cancellationToken)
           .ConfigureAwait(false);

        return await GetResultAsync<GameRoundViewModel>(
            responseMessage: response,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["gameProviderId"] = gameProviderId.ToString();
        query["externalRoundId"] = externalRoundId;
        query["gameId"] = gameId.ToString();
        query["walletId"] = walletId.ToString();

        var requestUri = $"gamerounds/game?{query}";

        var response = await _httpClient
            .GetAsync(
                requestUri: requestUri,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResultAsync<GameRoundViewModel>(
            responseMessage: response,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<GameRoundDetailViewModel?> GetRoundDetailAsync(Guid roundId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
            .GetAsync(
                requestUri: $"gamerounds/{roundId}/getdetail",
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResultAsync<GameRoundDetailViewModel>(
            responseMessage: response,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    private static async Task<TResult?> GetResultAsync<TResult>(HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        var content = await responseMessage.Content.ReadAsStringAsync(
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(content))
        {
            return default;
        }

        if (responseMessage.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<TResult?>(content, _SerializerOptions);
        }

        if (responseMessage.StatusCode == HttpStatusCode.InternalServerError)
        {
            try
            {
                return JsonSerializer.Deserialize<TResult?>(content, _SerializerOptions);
            }
            catch
            {
                return default;
            }
        }

        return default;
    }
}
