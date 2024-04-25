using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRounds.ViewModels;

public record RoundByExternalGameViewModel
{
	[Required]
	public Guid GameProviderId { get; init; }
	[Required]
	public Guid WalletId { get; init; }
	[Required]
	public string ExternalRoundId { get; init; } = default!;
	[Required]
	public string ExternalGameId { get; init; } = default!;

	public DateTimeOffset? ExternalCreatedOn { get; init; }
}
