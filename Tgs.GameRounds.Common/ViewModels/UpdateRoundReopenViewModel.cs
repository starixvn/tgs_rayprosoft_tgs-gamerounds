using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRounds.Common.ViewModels;

public record UpdateRoundReopenViewModel
{
	public GameRoundStatus Status { get; init; }
	public DateTimeOffset? ExternalClosedOn { get; init; }
	[Required]
	[Range(minimum: 0, maximum: int.MaxValue)]
	public decimal BetAmount { get; init; }
	[Required]
	[Range(minimum: 0, maximum: int.MaxValue)]
	public decimal ValidBetAmount { get; init; }
	[Required]
	[Range(minimum: 0, maximum: int.MaxValue)]
	public decimal SettleAmount { get; init; }
	[Required]
	[Range(minimum: 0, maximum: int.MaxValue)]
	public decimal Commission { get; init; }
	[Required]
	[Range(minimum: 0, maximum: int.MaxValue)]
	public decimal Turnover { get; init; }
	public decimal PlayerWinLoss { get; init; }
	public GameRoundClientStatus ClientStatus { get; init; }
	public string AdditionalData { get; init; } = default!;
}
