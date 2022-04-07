namespace Tgs.GameRounds.Common.ViewModels
{
    public record UpdateRoundReopenViewModel(
        GameRoundStatus Status,
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
