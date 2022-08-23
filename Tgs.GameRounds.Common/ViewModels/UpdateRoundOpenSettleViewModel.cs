using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRounds.Common.ViewModels;

public record UpdateRoundOpenSettleViewModel
{
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
	[Required]
	[Range(minimum: 0, maximum: int.MaxValue)]
	public decimal PlayerWinLoss { get; init; }
}
