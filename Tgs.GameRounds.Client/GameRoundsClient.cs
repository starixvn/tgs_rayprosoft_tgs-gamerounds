using System.Net;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
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
       => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public async ValueTask<GameRoundResponse> AddCompanyRoundAsync(AddCompanyRoundViewModel model, CancellationToken cancellationToken = default)
    {
        var requestContent = new StringContent(
            content: JsonSerializer.Serialize(model, _SerializerOptions),
            encoding: Encoding.UTF8,
            mediaType: MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PostAsync(
               requestUri: "gamerounds/company",
               content: requestContent,
               cancellationToken: cancellationToken)
           .ConfigureAwait(false);

        return new GameRoundResponse(
                Success: response.IsSuccessStatusCode,
                HttpStatusCode: response.StatusCode);
    }

    public async ValueTask<GameRoundResponse> AdjustClosedRoundAsync(Guid roundId, AdjustClosedRoundViewModel model, CancellationToken cancellationToken = default)
    {
        var requestContent = new StringContent(
            content: JsonSerializer.Serialize(model, _SerializerOptions),
            encoding: Encoding.UTF8,
            mediaType: MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PutAsync(
              requestUri: $"gamerounds/{roundId}/adjust/closed",
              content: requestContent,
              cancellationToken: cancellationToken)
           .ConfigureAwait(false);

        return new GameRoundResponse(
                Success: response.IsSuccessStatusCode,
                HttpStatusCode: response.StatusCode);
    }

    public async ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid roundId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
            .GetAsync(
                requestUri: $"gamerounds/{roundId}",
                cancellationToken: cancellationToken)
           .ConfigureAwait(false);

        return new GameRoundDataResponse(
                Success: response.IsSuccessStatusCode,
                HttpStatusCode: response.StatusCode,
                Data: await GetResultAsync<GameRoundViewModel>(
                    responseMessage: response,
                    cancellationToken: cancellationToken).ConfigureAwait(false));
    }

    public async ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default)
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

        return new GameRoundDataResponse(
                Success: response.IsSuccessStatusCode,
                HttpStatusCode: response.StatusCode,
                Data: await GetResultAsync<GameRoundViewModel>(
                    responseMessage: response,
                    cancellationToken: cancellationToken).ConfigureAwait(false));
    }

    public async ValueTask<GameRoundDetailDataResponse> GetRoundDetailAsync(Guid roundId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
            .GetAsync(
                requestUri: $"gamerounds/{roundId}/getdetail",
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new GameRoundDetailDataResponse(
                Success: response.IsSuccessStatusCode,
                HttpStatusCode: response.StatusCode,
                Data: await GetResultAsync<GameRoundDetailViewModel>(
                    responseMessage: response,
                    cancellationToken: cancellationToken).ConfigureAwait(false));
    }

    public async ValueTask<GameRoundResponse> ManualCloseRoundAsync(Guid roundId, ManualCloseRoundViewModel model, CancellationToken cancellationToken = default)
    {
        var requestContent = new StringContent(
            content: JsonSerializer.Serialize(model, _SerializerOptions),
            encoding: Encoding.UTF8,
            mediaType: MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PutAsync(
              requestUri: $"gamerounds/{roundId}/manual/closed",
              content: requestContent,
              cancellationToken: cancellationToken)
           .ConfigureAwait(false);

        return new GameRoundResponse(
                Success: response.IsSuccessStatusCode,
                HttpStatusCode: response.StatusCode);
    }

    public async ValueTask<GameRoundResponse> UpdateRoundDetailAsync(Guid roundId, string additionalData, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            additionalData
        };

        var requestContent = new StringContent(
            content: JsonSerializer.Serialize(request, _SerializerOptions),
            encoding: Encoding.UTF8,
            mediaType: MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PutAsync(
              requestUri: $"gamerounds/{roundId}/detail",
              content: requestContent,
              cancellationToken: cancellationToken)
           .ConfigureAwait(false);

        return new GameRoundResponse(
                Success: response.IsSuccessStatusCode,
                HttpStatusCode: response.StatusCode);
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
