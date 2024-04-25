using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Entities;

namespace Tgs.GameRounds.Repositories;
internal class RoundCacheLockedRepository(
	IRoundRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<RoundCacheLockedRepository> logger) : IRoundRepository
{
	public async ValueTask AddAsync(RoundAdd source, CancellationToken cancellationToken = default)
	{
		var key = GetKey(gameProviderId: source.GameProviderId, gameId: source.GameId, walletId: source.WalletId, externalRoundId: source.ExternalRoundId);
		try
		{
			if (!await database.StringSetAsync(
				key: key,
				value: new JsonObject
				{
					["GameProviderId"] = source.GameProviderId,
					["GameId"] = source.GameId,
					["PlayerCode"] = source.PlayerCode,
					["WalletId"] = source.WalletId,
					["ExternalRoundId"] = source.ExternalRoundId,
					["ExternalCreatedOn"] = source.ExternalCreatedOn,
					["LockedOn"] = DateTimeOffset.UtcNow
				}.ToJsonString(),
				expiry: expiry,
				when: When.NotExists).ConfigureAwait(false))
			{
				return;
			}

			await repository.AddAsync(
				source: source,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn} Message:An exception has occurred when adding bets to this game..{info}",
				DateTime.Now.ToString("o"),
				new
				{
					source.GameProviderId,
					source.GameId,
					source.PlayerCode,
					source.WalletId,
					source.ExternalRoundId,
					source.ExternalCreatedOn,
				});

			await database.KeyDeleteAsync(
				key: key,
				flags: CommandFlags.FireAndForget)
				.ConfigureAwait(false);

			throw;
		}
	}
	public ValueTask ClosedAsync(Guid id, RoundClosed source, CancellationToken cancellationToken = default)
		=> repository.ClosedAsync(id, source, cancellationToken);
	public ValueTask<RoundInfo?> GetAsync(Guid id, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetAsync(id, externalCreatedOn, cancellationToken);
	public ValueTask<RoundInfo?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetAsync(gameProviderId, gameId, walletId, externalRoundId, externalCreatedOn, cancellationToken);
	public ValueTask<Guid?> GetIdAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default)
		=> repository.GetIdAsync(gameProviderId, gameId, walletId, externalRoundId, externalCreatedOn, cancellationToken);
	public ValueTask<RoundDetailInfo?> GetRoundDetailAsync(Guid id, CancellationToken cancellationToken = default)
		=> repository.GetRoundDetailAsync(id, cancellationToken);
	public ValueTask MergeDetailAsync(Guid id, string additionalData, CancellationToken cancellationToken = default)
		=> repository.MergeDetailAsync(id, additionalData, cancellationToken);
	public ValueTask OpenAsync(Guid id, RoundOpen source, CancellationToken cancellationToken = default)
		=> repository.OpenAsync(id, source, cancellationToken);
	public ValueTask ReopenAsync(Guid id, RoundReopen source, CancellationToken cancellationToken = default)
		=> repository.ReopenAsync(id, source, cancellationToken);

	private static string GetKey(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId)
		=> string.Concat(typeof(RoundCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "locked", ":", gameProviderId, ":", gameId, ":", walletId, ":", externalRoundId);

}
