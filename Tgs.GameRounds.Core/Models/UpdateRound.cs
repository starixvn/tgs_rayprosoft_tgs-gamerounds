namespace Tgs.GameRounds.Core.Models
{
    public record UpdateRound(
          Guid RoundId,
          RoundStatus Status,
          DateTimeOffset? ClosedOn,
          DateTimeOffset? ExternalClosedOn,
          decimal? BetAmount,
          decimal? ValidBetAmount,
          decimal? SettleAmount,
          decimal? Commission,
          decimal? Turnover,
          decimal? WinLoss,
          decimal? TieBetAdjustment,
          RoundClientStatus ClientStatus,
          decimal? ReopenTurnoverAdjustment,
          DateTimeOffset? ReopenedOn,
          string? AdditionalData);
}
