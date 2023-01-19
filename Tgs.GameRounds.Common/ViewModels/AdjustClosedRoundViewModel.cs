using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRounds.Common.ViewModels;

public record AdjustClosedRoundViewModel
{
    [Required]
    public DateTimeOffset ExternalClosedOn { get; init; }
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
    public decimal CompanyWinLoss { get; init; }
    [Required]
    public GameRoundClientStatus ClientStatus { get; init; }
    [Required]
    public Guid OperatorId { get; init; }
    [Required]
    public string Remark { get; init; } = default!;
}
