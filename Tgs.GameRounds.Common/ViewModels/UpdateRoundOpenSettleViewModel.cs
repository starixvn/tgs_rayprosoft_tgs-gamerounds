namespace Tgs.GameRounds.Common.ViewModels
{
    public record UpdateRoundOpenSettleViewModel(
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal SettleAmount,
        decimal Commission,
        decimal Turnover,
        decimal PlayerWinLoss);
}
