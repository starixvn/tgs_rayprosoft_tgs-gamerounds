using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

public interface IGameRoundsClient
{
	ValueTask<Guid> RoundCommandAsync(RoundCommandViewModel source, CancellationToken cancellationToken = default);

	ValueTask<RoundViewModel?> RoundGetAsync(Guid roundId, CancellationToken cancellationToken = default);

	ValueTask<RoundViewModel?> RoundGetAsync(Guid roundId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask<RoundViewModel?> RoundGetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, CancellationToken cancellationToken = default);

	ValueTask<RoundViewModel?> RoundGetAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask<RoundViewModel?> RoundGetAsync(Guid gameProviderId, Guid walletId, string externalRoundId, string externalGameId, CancellationToken cancellationToken = default);

	ValueTask<RoundViewModel?> RoundGetAsync(Guid gameProviderId, Guid walletId, string externalRoundId, string externalGameId, DateTimeOffset externalCreatedOn, CancellationToken cancellationToken = default);

	ValueTask<RoundDetailViewModel?> RoundDetailGetAsync(Guid roundId, CancellationToken cancellationToken = default);
}
