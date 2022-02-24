using Tgs.DataBase.Round.Core.Models;

namespace Tgs.DataBase.Round.Core
{
    public interface IRoundRepository
    {
        ValueTask ClosedRoundAsync(ClosedRound round);

        ValueTask<RoundInfo?> GetAsync(Guid roundId);

        ValueTask<RoundInfo?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<Guid?> NewRoundAsync(NewRound round);

        ValueTask OpenRoundAsync(OpenRound round);

        ValueTask UpdateRoundAsync(UpdateRound round);

        ValueTask ReopenedRoundAsync(ReopenedRound round);
    }
}
