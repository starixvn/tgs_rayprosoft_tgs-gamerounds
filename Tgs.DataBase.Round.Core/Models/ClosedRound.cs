namespace Tgs.DataBase.Round.Core.Models
{
    public record ClosedRound(
          Guid RoundId,
          decimal BetAmount,
          decimal ValidBetAmount,
          decimal SettleAmount,
          decimal Commission,
          decimal Turnover,
          decimal WinLoss,
          DateTimeOffset ExternalClosedOn,
          RoundClientStatus ClientStatus);
}
