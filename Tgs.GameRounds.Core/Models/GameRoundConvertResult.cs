namespace Tgs.GameRounds.Core.Models
{
    public record GameRoundConvertResult(
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal SettleAmount,
        decimal Commission,
        decimal Turnover,
        decimal PlayerWinLoss);
}
