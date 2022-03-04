namespace Tgs.GameRounds.Common.ViewModels
{
    public record NewRoundResultViewModel(
        bool Success = false,
        Guid? RoundId = null);
}
