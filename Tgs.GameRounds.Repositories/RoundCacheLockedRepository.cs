using System.Text.Json;
using System.Text.Json.Nodes;
using MediatR;
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
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					source.Id,
					source.GameProviderId,
					source.GameId,
					source.PlayerCode,
					source.WalletId,
					source.ExternalRoundId,
					ExternalCreatedOn = source.ExternalCreatedOn.ToString("u"),
					Status = source.Status.ToString(),
					source.Bet,
					source.ValidBet,
					source.Settle,
					source.Commission,
					source.PlayerWinLoss,
					Message = "An exception occurred while adding a new round to this game."
				}));

			await database.KeyDeleteAsync(
				key: key,
				flags: CommandFlags.FireAndForget)
				.ConfigureAwait(false);

			throw;
		}
	}

	public ValueTask ClosedAsync(DateTimeOffset externalCreatedOn, Guid id, RoundClosed source, CancellationToken cancellationToken = default)
		=> repository.ClosedAsync(externalCreatedOn, id, source, cancellationToken);

	public ValueTask<RoundInfo?> GetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default)
		=> repository.GetAsync(externalCreatedOn, id, cancellationToken);

	public ValueTask<Guid?> GetIdAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, CancellationToken cancellationToken = default)
		=> repository.GetIdAsync(externalCreatedOn, gameProviderId, gameId, walletId, externalRoundId, cancellationToken);

	public ValueTask OpenAsync(DateTimeOffset externalCreatedOn, Guid id, RoundOpen source, CancellationToken cancellationToken = default)
		=> repository.OpenAsync(externalCreatedOn, id, source, cancellationToken);

	public ValueTask ReopenAsync(DateTimeOffset externalCreatedOn, Guid id, RoundReopen source, CancellationToken cancellationToken = default)
		=> repository.ReopenAsync(externalCreatedOn, id, source, cancellationToken);

	private static string GetKey(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId)
		=> string.Concat(typeof(RoundCacheLockedRepository).Namespace!.Replace('.', '-').ToLower(), "round", ":", "locked", ":", gameProviderId, ":", gameId, ":", walletId, ":", externalRoundId);
}
