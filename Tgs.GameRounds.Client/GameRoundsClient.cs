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

        public async ValueTask AddRoundAsync(AddRoundViewModel model)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(model, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   $"gamerounds",
                   requestContent)
               .ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();
        }

        public async ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid rounid)
        {
            var response = await _httpClient
                .GetAsync($"gamerounds/{rounid}")
                .ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return (string.IsNullOrEmpty(contentJson)) ? null : JsonSerializer.Deserialize<GameRoundViewModel?>(contentJson, _SerializerOptions);
        }

        public async ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, string externalGameId, Guid walletId)
        {
            var response = await _httpClient
                .GetAsync($"gamerounds/externalgame?gameProviderId={gameProviderId}&externalRoundId={externalRoundId}&externalGameId={externalGameId}&walletId={walletId}")
                .ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return (string.IsNullOrEmpty(contentJson)) ? null : JsonSerializer.Deserialize<GameRoundViewModel?>(contentJson, _SerializerOptions);
        }

        public async ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
        {
            var response = await _httpClient
                .GetAsync($"gamerounds/game?gameProviderId={gameProviderId}&externalRoundId={externalRoundId}&gameId={gameId}&walletId={walletId}")
                .ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return (string.IsNullOrEmpty(contentJson)) ? null : JsonSerializer.Deserialize<GameRoundViewModel?>(contentJson, _SerializerOptions);
        }

        public async ValueTask<GameRoundStatus?> GetStatusAsync(Guid rounid)
        {
            var response = await _httpClient
                .GetAsync($"gamerounds/{rounid}/status")
                .ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return (string.IsNullOrEmpty(contentJson)) ? null : JsonSerializer.Deserialize<GameRoundStatus?>(contentJson, _SerializerOptions);
        }

        public async ValueTask<NewRoundResultViewModel?> NewRoundAsync(NewRoundQueryViewModel model)
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

        public async ValueTask<UpdateRoundResultViewModel?> UpdateRoundAsync(Guid roundId, UpdateRoundQueryViewModel model)
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

        public async ValueTask UpdateRoundClosedAsync(Guid roundId, UpdateRoundClosedViewModel model)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(model, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"gamerounds/{roundId}/closed",
                   requestContent)
               .ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();
        }

        public async ValueTask UpdateRoundOpenAsync(Guid roundId, UpdateRoundOpenViewModel model)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(model, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"gamerounds/{roundId}/open",
                   requestContent)
               .ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();
        }

        public async ValueTask UpdateRoundReopenAsync(Guid roundId, UpdateRoundReopenViewModel model)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(model, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"gamerounds/{roundId}/reopen",
                   requestContent)
               .ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();
        }
    }
}
