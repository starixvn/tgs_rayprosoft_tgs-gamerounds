using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

internal class FailedGameRoundsClient : IFailedGameRoundsClient
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

    public FailedGameRoundsClient(HttpClient httpClient)
       => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public async ValueTask AddAsync(
        AddFailedGameRoundViewModel source,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new StringContent(
            content: JsonSerializer.Serialize(source, _SerializerOptions),
            encoding: Encoding.UTF8,
            mediaType: MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PostAsync(
               requestUri: "failedgameround",
               content: requestContent,
               cancellationToken: cancellationToken)
           .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask<FailedGameRoundInfoViewModel?> GetAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        string playerCode,
        CancellationToken cancellationToken = default)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["gameProviderId"] = gameProviderId.ToString();
        query["externalRoundId"] = externalRoundId;
        query["gameId"] = gameId.ToString();
        query["playerCode"] = playerCode;

        var requestUri = $"failedgameround/external?{query}";

        var response = await _httpClient
            .GetAsync(
                requestUri: requestUri,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        return response.StatusCode == HttpStatusCode.NoContent
            ? null
            : await response
                .Content
                .ReadFromJsonAsync<FailedGameRoundInfoViewModel>(
                    _SerializerOptions,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }
}
