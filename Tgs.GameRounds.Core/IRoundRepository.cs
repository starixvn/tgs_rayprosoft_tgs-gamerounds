namespace Tgs.GameRounds.Core;

public interface IRoundRepository
{
	ValueTask<Models.RoundDetailInfo?> GetRoundDetailAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask AddAsync(Models.RoundAdd source, CancellationToken cancellationToken = default);

	ValueTask ClosedAsync(DateTimeOffset externalCreatedOn, Guid id, Models.RoundClosed source, CancellationToken cancellationToken = default);

	ValueTask<Models.RoundInfo?> GetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default);

	ValueTask<Models.RoundInfo?> GetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, CancellationToken cancellationToken = default);

	ValueTask<Guid?> GetIdAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, CancellationToken cancellationToken = default);

	ValueTask MergeDetailAsync(Guid id, string additionalData, CancellationToken cancellationToken = default);

	ValueTask OpenAsync(DateTimeOffset externalCreatedOn, Guid id, Models.RoundOpen source, CancellationToken cancellationToken = default);

	ValueTask ReopenAsync(DateTimeOffset externalCreatedOn, Guid id, Models.RoundReopen source, CancellationToken cancellationToken = default);
}
