using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

internal class GameRoundsCheckingClient : IGameRoundsCheckingClient
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

    public GameRoundsCheckingClient(HttpClient httpClient)
        => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public async ValueTask<GameRoundCheckingStatus> GetCheckingStatusAsync(
        GameRoundCheckingViewModel source,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new StringContent(
            content: JsonSerializer.Serialize(source, _SerializerOptions),
            encoding: Encoding.UTF8,
            mediaType: MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PostAsync(
               requestUri: "checking/",
               content: requestContent,
               cancellationToken: cancellationToken)
           .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<GameRoundCheckingStatus>(
                await response.Content
                .ReadAsStringAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
                _SerializerOptions);
    }
}
