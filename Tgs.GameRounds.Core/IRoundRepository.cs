namespace Tgs.GameRounds.Core;

public interface IRoundRepository
{
	ValueTask<Models.RoundDetailInfo?> GetRoundDetailAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask AddAsync(Models.RoundAdd source, CancellationToken cancellationToken = default);

	ValueTask ClosedAsync(Guid id, Models.RoundClosed source, CancellationToken cancellationToken = default);

	ValueTask<Models.RoundInfo?> GetAsync(Guid id, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask<Models.RoundInfo?> GetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask<Guid?> GetIdAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask MergeDetailAsync(Guid id, string additionalData, CancellationToken cancellationToken = default);

	ValueTask OpenAsync(Guid id, Models.RoundOpen source, CancellationToken cancellationToken = default);

	ValueTask ReopenAsync(Guid id, Models.RoundReopen source, CancellationToken cancellationToken = default);
}
