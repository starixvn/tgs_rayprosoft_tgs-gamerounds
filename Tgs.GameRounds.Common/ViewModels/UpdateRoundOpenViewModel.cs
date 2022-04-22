namespace Tgs.GameRounds.Common.ViewModels
{
    public record UpdateRoundOpenViewModel(
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal Commission,
        decimal Turnover);
}
