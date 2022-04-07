using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core
{
    public interface IGameRoundRepository
    {
        ValueTask<Guid> AddRoundAsync(AddRound source);

        ValueTask<GameRoundInfo> GetAsync(Guid roundId);

        ValueTask<GameRoundInfo?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<Guid?> NewRoundAsync(NewRound round);

        ValueTask UpdateRoundAsync(UpdateRound round);

        ValueTask UpdateRoundClosedAsync(UpdateRoundClosed source);

        ValueTask UpdateRoundDetailAsync(Guid roundId, string additionalData);

        ValueTask UpdateRoundOpenAsync(UpdateRoundOpen source);

        ValueTask UpdateRoundReopenAsync(UpdateRoundReopen source);
    }
}
