namespace Tgs.DataBase.Round.Core.Models
{
    public record UpdateRound(
          Guid RoundId,
          decimal BetAmount,
          decimal ValidBetAmount,
          decimal SettleAmount,
          decimal Commission,
          decimal Turnover,
          decimal WinLoss);
}
