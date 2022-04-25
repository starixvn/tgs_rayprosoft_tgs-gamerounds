using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core
{
    public interface IGameRound
    {
        ValueTask<Guid?> AddRoundAsync(AddRound source);

        ValueTask<bool> ExistAsync(Guid roundId);

        ValueTask<GameRoundInfo?> GetAsync(Guid roundId);

        ValueTask<GameRoundInfo?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<Guid?> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<bool> LockAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<Guid?> NewRoundAsync(NewRound round);

        ValueTask UnLockAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask UpdateClientStatusAsync(Guid roundId, GameRoundClientStatus status);

        ValueTask UpdateRoundClosedAsync(UpdateRoundClosed source);

        ValueTask UpdateRoundDetailAsync(Guid roundId, string additionalData);

        ValueTask UpdateRoundOpenAsync(UpdateRoundOpen source);

        ValueTask UpdateRoundOpenSettleAsync(UpdateRoundOpenSettle source);

        ValueTask UpdateRoundReopenAsync(UpdateRoundReopen source);

        ValueTask UpdateStatusAsync(Guid roundId, GameRoundStatus status, DateTimeOffset? externalClosedOn = null);
    }
}
