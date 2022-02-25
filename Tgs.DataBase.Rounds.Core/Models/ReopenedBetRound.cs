namespace Tgs.DataBase.Rounds.Core.Models
{
    public record ReopenedBetRound(
        Guid RoundId,
        decimal BetAmount,
        decimal ValidBetAmount);
}
