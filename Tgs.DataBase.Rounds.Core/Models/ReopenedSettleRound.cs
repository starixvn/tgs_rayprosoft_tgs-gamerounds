namespace Tgs.DataBase.Rounds.Core.Models
{
    public record ReopenedSettleRound(
          Guid RoundId,
          RoundStatus Status,
          decimal BetAmount,
          decimal ValidBetAmount,
          decimal SettleAmount,
          decimal Commission,
          decimal Turnover,
          decimal WinLoss,
          RoundClientStatus ClientStatus);
}
