namespace Tgs.GameRounds.Common.ViewModels;

public record RoundDetailViewModel
{
	public Guid RoundId { get; init; }
	public string AdditionalData { get; init; } = default!;
}
