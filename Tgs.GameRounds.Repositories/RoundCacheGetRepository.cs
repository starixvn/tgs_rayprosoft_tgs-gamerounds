using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Repositories;

internal class RoundCacheGetRepository(
	IRoundRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<RoundCacheGetRepository> logger) : IRoundRepository
{
	private static readonly JsonSerializerOptions _SerializerOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};

	public ValueTask AddAsync(RoundAdd source, CancellationToken cancellationToken = default)
		=> repository.AddAsync(source, cancellationToken);

	public async ValueTask ClosedAsync(DateTimeOffset externalCreatedOn, Guid id, RoundClosed source, CancellationToken cancellationToken = default)
	{
		try
		{
			await repository.ClosedAsync(externalCreatedOn, id, source, cancellationToken).ConfigureAwait(false);
			await KeyDeleteAsync(id).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Id = id,
					ExternalCreatedOn = externalCreatedOn.ToString("u"),
					Message = "This exception occurs."
				}));

			throw;
		}
	}

	public async ValueTask<RoundInfo?> GetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default)
	{
		try
		{
			var info = await StringGetAsync(id).ConfigureAwait(false)
				?? await repository.GetAsync(externalCreatedOn, id, cancellationToken).ConfigureAwait(false);

			if (info is not null)
			{
				await StringSetAsync(info).ConfigureAwait(false);
			}

			return info;
		}
		catch (RedisCommandException ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Id = id,
					ExternalCreatedOn = externalCreatedOn.ToString("u"),
					Message = "This exception occurs."
				}));

			return await repository.GetAsync(externalCreatedOn, id, cancellationToken).ConfigureAwait(false);
		}
		catch (RedisTimeoutException ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Id = id,
					ExternalCreatedOn = externalCreatedOn.ToString("u"),
					Message = "This exception occurs."
				}));

			return await repository.GetAsync(externalCreatedOn, id, cancellationToken).ConfigureAwait(false);
		}
		catch (RedisException ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Id = id,
					ExternalCreatedOn = externalCreatedOn.ToString("u"),
					Message = "This exception occurs."
				}));

			return await repository.GetAsync(externalCreatedOn, id, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Id = id,
					ExternalCreatedOn = externalCreatedOn.ToString("u"),
					Message = "This exception occurs."
				}));

			throw;
		}
	}

	public ValueTask<Guid?> GetIdAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, CancellationToken cancellationToken = default)
		=> repository.GetIdAsync(externalCreatedOn, gameProviderId, gameId, walletId, externalRoundId, cancellationToken);

	public async ValueTask OpenAsync(DateTimeOffset externalCreatedOn, Guid id, RoundOpen source, CancellationToken cancellationToken = default)
	{
		try
		{
			await repository.OpenAsync(externalCreatedOn, id, source, cancellationToken).ConfigureAwait(false);
			await KeyDeleteAsync(id).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Id = id,
					ExternalCreatedOn = externalCreatedOn.ToString("u"),
					Message = "This exception occurs."
				}));

			throw;
		}
	}

	public async ValueTask ReopenAsync(DateTimeOffset externalCreatedOn, Guid id, RoundReopen source, CancellationToken cancellationToken = default)
	{
		try
		{
			await repository.ReopenAsync(externalCreatedOn, id, source, cancellationToken).ConfigureAwait(false);
			await KeyDeleteAsync(id).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Id = id,
					ExternalCreatedOn = externalCreatedOn.ToString("u"),
					Message = "This exception occurs."
				}));

			throw;
		}
	}

	private static string GetKey(Guid id)
		=> string.Concat(typeof(RoundCacheGetRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "get", ":", ":", "id", ":", id);

	private async ValueTask KeyDeleteAsync(Guid id)
	{
		var key = GetKey(id);

		_ = await database.KeyDeleteAsync(
			key: key,
			flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);
	}

	private async ValueTask StringSetAsync(RoundInfo info)
	{
		var key = GetKey(info.Id);

		_ = await database.StringSetAsync(
			key: key,
			value: JsonSerializer.Serialize(info, _SerializerOptions),
			expiry: expiry,
			When.NotExists,
			flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);
	}

	private async ValueTask<RoundInfo?> StringGetAsync(Guid id)
	{
		var key = GetKey(id);

		if (await database.KeyExistsAsync(key).ConfigureAwait(false))
		{
			var redisValue = await database.StringGetAsync(key).ConfigureAwait(false);

			return string.IsNullOrWhiteSpace(redisValue) ? null : JsonSerializer.Deserialize<RoundInfo?>(redisValue!, _SerializerOptions);
		}

		return null;
	}
}
