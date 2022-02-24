namespace Tgs.DataBase.Round.Core.Models
{
    public record ReopenedRound(
          Guid RoundId,
          decimal BetAmount,
          decimal ValidBetAmount,
          decimal SettleAmount,
          decimal Commission,
          decimal Turnover,
          decimal WinLoss,
          RoundClientStatus ClientStatus);
}
