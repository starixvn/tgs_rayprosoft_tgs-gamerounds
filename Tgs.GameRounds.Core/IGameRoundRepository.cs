using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core;

public interface IGameRoundRepository
{
    ValueTask<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default);

    ValueTask<GameRoundInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    ValueTask<GameRoundInfo?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default);

    ValueTask<GameRoundDetailInfo?> GetRoundDetailAsync(Guid id, CancellationToken cancellationToken = default);

    ValueTask<Guid?> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default);
}
