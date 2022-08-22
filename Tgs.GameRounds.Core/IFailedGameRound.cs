using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core;

public interface IFailedGameRound
{
	Task AddAsync(
		AddFailedGameRound source,
		CancellationToken cancellationToken = default);

	Task<FailedGameRoundInfo?> GetAsync(
		Guid gameProviderId,
		string externalRoundId,
		int gameId,
		string playerCode,
		CancellationToken cancellationToken = default);
}
