namespace Tgs.GameRounds.Common.ViewModels
{
    public record AddRoundBatchsViewModel
    {
        public IEnumerable<AddRoundBatchViewModel> AddRoundBatches { get; init; } = default!;
    }
}
