using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRounds.Common.ViewModels
{
    public record AddCompanyRoundViewModel
    {
        [Required]
        public string ExternalRoundId { get; init; } = default!;
        [Required]
        public Guid GameProviderId { get; init; }
        [Required]
        public string PlayerCode { get; init; } = default!;
        [Required]
        public int GameId { get; init; }
        [Required]
        public string CurrencyCode { get; init; } = default!;
        [Required]
        public Guid GameProviderConfigurationId { get; init; }
        [Required]
        public Guid WalletId { get; init; }
        [Required]
        public Guid BrandId { get; init; }
        [Required]
        public DateTimeOffset ExternalCreatedOn { get; init; }
        public DateTimeOffset ExternalClosedOn { get; init; }
        public decimal BetAmount { get; init; }
        public decimal ValidBetAmount { get; init; }
        public decimal SettleAmount { get; init; }
        public decimal Commission { get; init; }
        public decimal Turnover { get; init; }
        public decimal CompanyWinLoss { get; init; }
        public GameRoundClientStatus ClientStatus { get; init; }
        public Guid OperatorId { get; init; }
        public string Remark { get; init; } = default!;
        public string? AdditionalData { get; init; }
    }
}
