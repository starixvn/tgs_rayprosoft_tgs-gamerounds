using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRounds.Common.ViewModels
{
    public record AdjustClosedRoundViewModel
    {
        [Required]
        public DateTimeOffset ExternalClosedOn { get; init; }
        [Required]
        public decimal BetAmount { get; init; }
        [Required]
        public decimal ValidBetAmount { get; init; }
        [Required]
        public decimal SettleAmount { get; init; }
        [Required]
        public decimal Commission { get; init; }
        [Required]
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
}
