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

        public async ValueTask<GameRoundResponse> AddRoundAsync(AddRoundViewModel model)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(model, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   $"gamerounds",
                   requestContent)
               .ConfigureAwait(false);

            return new GameRoundResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode);
        }

        public async ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid rounid)
        {
            var response = await _httpClient
                .GetAsync($"gamerounds/{rounid}")
                .ConfigureAwait(false);

            return new GameRoundDataResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode,
                    Data: await GetResultAsync<GameRoundViewModel>(response).ConfigureAwait(false));
        }

        public async ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, string externalGameId, Guid walletId)
        {
            var response = await _httpClient
                .GetAsync($"gamerounds/externalgame?gameProviderId={gameProviderId}&externalRoundId={externalRoundId}&externalGameId={externalGameId}&walletId={walletId}")
                .ConfigureAwait(false);

            return new GameRoundDataResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode,
                    Data: await GetResultAsync<GameRoundViewModel>(response).ConfigureAwait(false));
        }

        public async ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId)
        {
            var response = await _httpClient
                .GetAsync($"gamerounds/game?gameProviderId={gameProviderId}&externalRoundId={externalRoundId}&gameId={gameId}&walletId={walletId}")
                .ConfigureAwait(false);

            return new GameRoundDataResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode,
                    Data: await GetResultAsync<GameRoundViewModel>(response).ConfigureAwait(false));
        }

        public async ValueTask<GameRoundRoundStatusResponse> GetStatusAsync(Guid rounid)
        {
            var response = await _httpClient
                .GetAsync($"gamerounds/{rounid}/status")
                .ConfigureAwait(false);

            return new GameRoundRoundStatusResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode,
                    RoundStatus: await GetResultAsync<GameRoundStatus>(response).ConfigureAwait(false));
        }

        public async ValueTask<GameRoundRoundIdResponse> NewRoundAsync(NewRoundQueryViewModel model)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(model, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "gamerounds/new",
                   requestContent)
               .ConfigureAwait(false);

            return new GameRoundRoundIdResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode,
                    RoundId: await GetResultAsync<Guid?>(response).ConfigureAwait(false));
        }

        public async ValueTask<GameRoundResponse> UpdateClientStatusAsync(Guid roundId, GameRoundClientStatus status)
        {
            var request = new
            {
                status
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"gamerounds/{roundId}/clientstatus",
                   requestContent)
               .ConfigureAwait(false);

            return new GameRoundResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode);
        }

        public async ValueTask<GameRoundResponse> UpdateRoundClosedAsync(Guid roundId, UpdateRoundClosedViewModel model)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(model, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"gamerounds/{roundId}/closed",
                   requestContent)
               .ConfigureAwait(false);

            return new GameRoundResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode);
        }

        public async ValueTask<GameRoundResponse> UpdateRoundDetailAsync(Guid roundId, string additionalData)
        {
            var request = new
            {
                additionalData
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"gamerounds/{roundId}/detail",
                   requestContent)
               .ConfigureAwait(false);

            return new GameRoundResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode);
        }

        public async ValueTask<GameRoundResponse> UpdateRoundOpenAsync(Guid roundId, UpdateRoundOpenViewModel model)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(model, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"gamerounds/{roundId}/open",
                   requestContent)
               .ConfigureAwait(false);

            return new GameRoundResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode);
        }

        public async ValueTask<GameRoundResponse> UpdateRoundOpenSettleAsync(Guid roundId, UpdateRoundOpenSettleViewModel model)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(model, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"gamerounds/{roundId}/open",
                   requestContent)
               .ConfigureAwait(false);

            return new GameRoundResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode);
        }

        public async ValueTask<GameRoundResponse> UpdateRoundReopenAsync(Guid roundId, UpdateRoundReopenViewModel model)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(model, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"gamerounds/{roundId}/reopen",
                   requestContent)
               .ConfigureAwait(false);

            return new GameRoundResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode);
        }

        public async ValueTask<GameRoundResponse> UpdateStatusAsync(Guid roundId, GameRoundStatus status, DateTimeOffset? externalClosedOn = null)
        {
            var request = new
            {
                status,
                externalClosedOn
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"gamerounds/{roundId}/status",
                   requestContent)
               .ConfigureAwait(false);

            return new GameRoundResponse(
                    Success: response.IsSuccessStatusCode,
                    HttpStatusCode: response.StatusCode);
        }

        private static async Task<TResult?> GetResultAsync<TResult>(HttpResponseMessage responseMessage)
        {
            var content = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(content))
            {
                return default;
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<TResult?>(content, _SerializerOptions);
            }

            if (responseMessage.StatusCode == System.Net.HttpStatusCode.InternalServerError)
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
}
