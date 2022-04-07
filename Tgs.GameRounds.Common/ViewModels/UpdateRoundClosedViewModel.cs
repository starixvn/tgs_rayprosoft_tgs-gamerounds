namespace Tgs.GameRounds.Common.ViewModels
{
    public record UpdateRoundClosedViewModel(
        DateTimeOffset? ClosedOn,
        DateTimeOffset UpdatedOn,
        DateTimeOffset? ExternalClosedOn,
        decimal? BetAmount,
        decimal? ValidBetAmount,
        decimal? SettleAmount,
        decimal? Commission,
        decimal? Turnover,
        decimal? WinLoss,
        decimal? TieBetAdjustment,
        GameRoundClientStatus ClientStatus,
        string AdditionalData);
}
