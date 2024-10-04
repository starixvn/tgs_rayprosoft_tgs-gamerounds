using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

internal class GameRoundsClient(HttpClient httpClient) : IGameRoundsClient
{
	private static readonly JsonSerializerOptions _SerializerOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};

	public async ValueTask<Guid> RoundCommandAsync(RoundCommandViewModel source, CancellationToken cancellationToken = default)
	{
		var path = new Uri(httpClient.BaseAddress!, $"gamerounds/command");
		var builder = new UriBuilder(path);
		var requestContent = JsonContent.Create(inputValue: source, options: _SerializerOptions);
		var response = await httpClient.PostAsync(
			builder.ToString(),
			requestContent,
			cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<Guid>(
			_SerializerOptions,
			cancellationToken).ConfigureAwait(false);
	}
}
