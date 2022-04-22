namespace Tgs.GameRounds.Core.Models
{
    public record UpdateRoundOpenSettle(
        Guid RoundId,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal SettleAmount,
        decimal Commission,
        decimal Turnover,
        decimal WinLoss);
}
