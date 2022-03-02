using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core
{
    public interface IRoundRepository
    {
        ValueTask<RoundInfo?> GetAsync(Guid roundId);

        ValueTask<RoundInfo?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<Guid?> NewRoundAsync(NewRound round);

        ValueTask UpdateRoundAsync(UpdateRound round);
    }
}
