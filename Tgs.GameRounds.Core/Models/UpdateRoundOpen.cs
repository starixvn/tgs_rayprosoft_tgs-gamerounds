namespace Tgs.GameRounds.Core.Models
{
    public record UpdateRoundOpen(
        Guid RoundId,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal Commission,
        decimal Turnover);
}
