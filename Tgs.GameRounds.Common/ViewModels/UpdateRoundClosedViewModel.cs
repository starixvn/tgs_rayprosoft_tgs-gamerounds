namespace Tgs.GameRounds.Common.ViewModels
{
    public record UpdateRoundClosedViewModel(
        DateTimeOffset ExternalClosedOn,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal SettleAmount,
        decimal Commission,
        decimal Turnover,
        decimal WinLoss,
        GameRoundClientStatus ClientStatus,
        string AdditionalData);
}
