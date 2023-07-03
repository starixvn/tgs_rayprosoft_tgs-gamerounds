namespace Tgs.GameRounds.Common.ViewModels;

public record GameRoundDetailViewModel
{
    public Guid RoundId { get; init; }
    public string AdditionalData { get; init; } = default!;
}
