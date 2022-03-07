using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRounds.Common.ViewModels
{
    public record UpdateRoundQueryViewModel()
    {
        [Required]
        [Range(minimum: (int)GameRoundStatus.Open, maximum: (int)GameRoundStatus.Closed)]
        public GameRoundStatus Status { get; init; }
        public DateTimeOffset? ClosedOn { get; init; }
        public DateTimeOffset? ExternalClosedOn { get; init; }
        public decimal? BetAmount { get; init; }
        public decimal? ValidBetAmount { get; init; }
        public decimal? SettleAmount { get; init; }
        public decimal? Commission { get; init; }
        public decimal? Turnover { get; init; }
        public decimal? WinLoss { get; init; }
        public decimal? TieBetAdjustment { get; init; }
        [Required]
        [Range(minimum: (int)GameRoundClientStatus.Open, maximum: (int)GameRoundClientStatus.Draw)]
        public GameRoundClientStatus ClientStatus { get; init; }
        public decimal? ReopenTurnoverAdjustment { get; init; }
        public DateTimeOffset? ReopenedOn { get; init; }
        public string? AdditionalData { get; init; }
    }
}
