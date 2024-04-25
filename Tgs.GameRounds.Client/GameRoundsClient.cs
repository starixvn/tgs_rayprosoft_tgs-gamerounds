using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
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

	public async ValueTask<RoundViewModel?> RoundGetAsync(Guid roundId, CancellationToken cancellationToken = default)
		=> await httpClient.GetAsync($"/gamerounds/{roundId}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<RoundViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<RoundViewModel?> RoundGetAsync(Guid roundId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
	{
		var path = new Uri(httpClient.BaseAddress!, $"gamerounds/{roundId}");
		var builder = new UriBuilder(path);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["externalCreatedOn"] = externalCreatedOn.ToString("u");
		builder.Query = query.ToString();
		string url = builder.ToString();

		return await httpClient.GetAsync(url, cancellationToken)
		 .ToAsyncEnumerable()
		 .Do(response => response.EnsureSuccessStatusCode())
		 .Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		 .SelectMany(response => response.Content
			 .ReadFromJsonAsync<RoundViewModel>(_SerializerOptions, cancellationToken)
			 .ToAsyncEnumerable())
		 .FirstOrDefaultAsync(cancellationToken: cancellationToken)
		 .ConfigureAwait(false);
		;
	}

	public async ValueTask<RoundViewModel?> RoundGetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, CancellationToken cancellationToken = default)
	{
		var path = new Uri(httpClient.BaseAddress!, $"gamerounds/game");
		var builder = new UriBuilder(path);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["gameProviderId"] = gameProviderId.ToString();
		query["externalRoundId"] = externalRoundId;
		query["gameId"] = gameId.ToString();
		query["walletId"] = walletId.ToString();
		builder.Query = query.ToString();
		string url = builder.ToString();

		return await httpClient.GetAsync(url, cancellationToken)
		 .ToAsyncEnumerable()
		 .Do(response => response.EnsureSuccessStatusCode())
		 .Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		 .SelectMany(response => response.Content
			 .ReadFromJsonAsync<RoundViewModel>(_SerializerOptions, cancellationToken)
			 .ToAsyncEnumerable())
		 .FirstOrDefaultAsync(cancellationToken: cancellationToken)
		 .ConfigureAwait(false);
		;
	}

	public async ValueTask<RoundViewModel?> RoundGetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
	{
		var path = new Uri(httpClient.BaseAddress!, $"gamerounds/game");
		var builder = new UriBuilder(path);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["gameProviderId"] = gameProviderId.ToString();
		query["gameId"] = gameId.ToString();
		query["walletId"] = walletId.ToString();
		query["externalRoundId"] = externalRoundId;
		query["externalCreatedOn"] = externalCreatedOn.ToString("u");
		builder.Query = query.ToString();
		string url = builder.ToString();

		return await httpClient.GetAsync(url, cancellationToken)
		 .ToAsyncEnumerable()
		 .Do(response => response.EnsureSuccessStatusCode())
		 .Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		 .SelectMany(response => response.Content
			 .ReadFromJsonAsync<RoundViewModel>(_SerializerOptions, cancellationToken)
			 .ToAsyncEnumerable())
		 .FirstOrDefaultAsync(cancellationToken: cancellationToken)
		 .ConfigureAwait(false);
		;
	}

	public async ValueTask<RoundViewModel?> RoundGetAsync(Guid gameProviderId, Guid walletId, string externalRoundId, string externalGameId, CancellationToken cancellationToken = default)
	{
		var path = new Uri(httpClient.BaseAddress!, $"gamerounds/external-game");
		var builder = new UriBuilder(path);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["gameProviderId"] = gameProviderId.ToString();
		query["externalRoundId"] = externalRoundId;
		query["externalGameId"] = externalGameId;
		query["walletId"] = walletId.ToString();
		builder.Query = query.ToString();
		string url = builder.ToString();

		return await httpClient.GetAsync(url, cancellationToken)
		 .ToAsyncEnumerable()
		 .Do(response => response.EnsureSuccessStatusCode())
		 .Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		 .SelectMany(response => response.Content
			 .ReadFromJsonAsync<RoundViewModel>(_SerializerOptions, cancellationToken)
			 .ToAsyncEnumerable())
		 .FirstOrDefaultAsync(cancellationToken: cancellationToken)
		 .ConfigureAwait(false);
		;
	}

	public async ValueTask<RoundViewModel?> RoundGetAsync(Guid gameProviderId, Guid walletId, string externalRoundId, string externalGameId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
	{
		var path = new Uri(httpClient.BaseAddress!, $"gamerounds/external-game");
		var builder = new UriBuilder(path);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["gameProviderId"] = gameProviderId.ToString();
		query["walletId"] = walletId.ToString();
		query["externalRoundId"] = externalRoundId;
		query["externalGameId"] = externalGameId;
		query["externalCreatedOn"] = externalCreatedOn.ToString("u");
		builder.Query = query.ToString();
		string url = builder.ToString();

		return await httpClient.GetAsync(url, cancellationToken)
		 .ToAsyncEnumerable()
		 .Do(response => response.EnsureSuccessStatusCode())
		 .Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		 .SelectMany(response => response.Content
			 .ReadFromJsonAsync<RoundViewModel>(_SerializerOptions, cancellationToken)
			 .ToAsyncEnumerable())
		 .FirstOrDefaultAsync(cancellationToken: cancellationToken)
		 .ConfigureAwait(false);
		;
	}

	public async ValueTask<RoundDetailViewModel?> RoundDetailGetAsync(Guid roundId, CancellationToken cancellationToken = default)
			=> await httpClient.GetAsync($"/gamerounds/{roundId}/getdetail", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<RoundDetailViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
