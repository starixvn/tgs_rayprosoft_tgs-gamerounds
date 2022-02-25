using Tgs.DataBase.Rounds.Core.Models;

namespace Tgs.DataBase.Rounds.Core
{
    public interface IRoundRepository
    {
        ValueTask ClosedRoundAsync(ClosedRound round);

        ValueTask<RoundInfo?> GetAsync(Guid roundId);

        ValueTask<RoundInfo?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<Guid?> NewRoundAsync(NewRound round);

        ValueTask OpenRoundAsync(OpenRound round);

        ValueTask ReopenedBetRoundAsync(ReopenedBetRound round);

        ValueTask ReopenedSettleRoundAsync(ReopenedSettleRound round);

        ValueTask TurnoverRoundAsync(TurnoverRound round);

        ValueTask UpdateRoundAsync(UpdateRound round);

        ValueTask VoidRoundAsync(VoidRound round);
    }
}
