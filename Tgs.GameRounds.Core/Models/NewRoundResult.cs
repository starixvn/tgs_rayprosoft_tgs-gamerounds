namespace Tgs.GameRounds.Core.Models
{
    public record NewRoundResult(
        bool Success = false,
        Guid? RoundId = null);
}
