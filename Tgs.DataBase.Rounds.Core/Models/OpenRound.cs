namespace Tgs.DataBase.Rounds.Core.Models
{
    public record OpenRound(
          Guid RoundId,
          decimal BetAmount,
          decimal ValidBetAmount);
}
