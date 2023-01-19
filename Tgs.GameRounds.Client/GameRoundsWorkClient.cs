using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

internal class GameRoundsWorkClient : IGameRoundsWorkClient
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

    public GameRoundsWorkClient(HttpClient httpClient)
        => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public async ValueTask ExecuteAsync(GameRoundsWorkViewModel model, CancellationToken cancellationToken = default)
    {
        var requestContent = new StringContent(
            content: JsonSerializer.Serialize(model, _SerializerOptions),
            encoding: Encoding.UTF8,
            mediaType: MediaTypeNames.Application.Json);

        var response = await _httpClient
           .PostAsync(
               requestUri: "gameroundswork",
               content: requestContent,
               cancellationToken: cancellationToken)
           .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }
}
