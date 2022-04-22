using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core
{
    internal class NoopGameRound : IGameRound
    {
        public ValueTask<Guid?> AddRoundAsync(AddRound source) => throw new NotSupportedException();

        public ValueTask<bool> ExistAsync(Guid roundId) => throw new NotSupportedException();

        public ValueTask<GameRoundInfo?> GetAsync(Guid roundId) => throw new NotSupportedException();

        public ValueTask<GameRoundInfo?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId) => throw new NotSupportedException();

        public ValueTask<Guid?> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId) => throw new NotSupportedException();

        public ValueTask<bool> LockAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId) => throw new NotSupportedException();

        public ValueTask<Guid?> NewRoundAsync(NewRound round) => throw new NotSupportedException();

        public ValueTask UnLockAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId) => throw new NotSupportedException();

        public ValueTask UpdateClientStatusAsync(Guid roundId, GameRoundClientStatus status) => throw new NotSupportedException();

        public ValueTask UpdateRoundAsync(UpdateRound source) => throw new NotSupportedException();

        public ValueTask UpdateRoundClosedAsync(UpdateRoundClosed source) => throw new NotSupportedException();

        public ValueTask UpdateRoundDetailAsync(Guid roundId, string additionalData) => throw new NotSupportedException();

        public ValueTask UpdateRoundOpenAsync(UpdateRoundOpen source) => throw new NotSupportedException();

        public ValueTask UpdateRoundOpenSettleAsync(UpdateRoundOpenSettle source) => throw new NotSupportedException();

        public ValueTask UpdateRoundReopenAsync(UpdateRoundReopen source) => throw new NotSupportedException();

        public ValueTask UpdateStatusAsync(Guid roundId, GameRoundStatus status, DateTimeOffset? externalClosedOn = null) => throw new NotSupportedException();
    }
}
