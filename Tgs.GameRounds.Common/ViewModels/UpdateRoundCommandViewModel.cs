namespace Tgs.GameRounds.Common.ViewModels
{
    public record UpdateRoundCommandViewModel(
          Guid RoundId,
          GameRoundStatus Status,
          DateTimeOffset? ClosedOn,
          DateTimeOffset? ExternalClosedOn,
          decimal? BetAmount,
          decimal? ValidBetAmount,
          decimal? SettleAmount,
          decimal? Commission,
          decimal? Turnover,
          decimal? WinLoss,
          decimal? TieBetAdjustment,
          GameRoundClientStatus ClientStatus,
          decimal? ReopenTurnoverAdjustment,
          DateTimeOffset? ReopenedOn,
          string? AdditionalData);
}
