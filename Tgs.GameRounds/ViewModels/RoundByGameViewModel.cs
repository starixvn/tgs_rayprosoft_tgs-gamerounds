using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRounds.ViewModels;

public record RoundByGameViewModel
{
	[Required]
	public Guid GameProviderId { get; init; }
	[Required]
	public int GameId { get; init; }
	[Required]
	public Guid WalletId { get; init; }
	[Required]
	public string ExternalRoundId { get; init; } = default!;
	public DateTimeOffset? ExternalCreatedOn { get; init; }
}
