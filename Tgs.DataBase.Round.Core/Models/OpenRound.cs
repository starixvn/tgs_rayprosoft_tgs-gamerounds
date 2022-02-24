namespace Tgs.DataBase.Round.Core.Models
{
    public record OpenRound(
          Guid RoundId,
          decimal BetAmount,
          decimal ValidBetAmount);
}
