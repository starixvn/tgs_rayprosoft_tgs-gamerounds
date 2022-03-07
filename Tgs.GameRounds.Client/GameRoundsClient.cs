using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client
{
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

        public async ValueTask<GameRoundStatus?> GameRoundGetStatus(Guid rounid)
        {
            var response = await _httpClient
                .GetAsync($"gamerounds/{rounid}/status")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return (string.IsNullOrEmpty(contentJson)) ? null : JsonSerializer.Deserialize<GameRoundStatus?>(contentJson, _SerializerOptions);
        }

        public async ValueTask<NewRoundResultViewModel?> GameRoundsNewRoundAsync(NewRoundQueryViewModel model)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(model, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "gamerounds/new",
                   requestContent)
               .ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<NewRoundResultViewModel>(contentJson, _SerializerOptions);
        }

        public async ValueTask<UpdateRoundResultViewModel?> GameRoundsUpdateRoundAsync(Guid roundId, UpdateRoundQueryViewModel model)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(model, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"gamerounds/{roundId}",
                   requestContent)
               .ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();
            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<UpdateRoundResultViewModel>(contentJson, _SerializerOptions);
        }
    }
}
