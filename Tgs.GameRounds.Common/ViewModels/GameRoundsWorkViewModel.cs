using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRounds.Common.ViewModels;
public record GameRoundsWorkViewModel
{
    [Required]
    public string ExternalRoundId { get; init; } = default!;
    [Required]
    public Guid GameProviderId { get; init; }
    [Required]
    public int GameId { get; init; }
    [Required]
    public string PlayerCode { get; init; } = default!;

    [Required]
    public string CurrencyCode { get; init; } = default!;
    [Required]
    public Guid WalletId { get; init; }

    [Required]
    public Guid BrandId { get; init; }
    [Required]
    public Guid GameProviderConfigurationId { get; init; }
    [Required]
    public GameRoundStatus Status { get; init; }
    [Required]
    public GameRoundClientStatus ClientStatus { get; init; }

    [Required]
    public DateTimeOffset ExternalCreatedOn { get; init; }
    public DateTimeOffset? ExternalClosedOn { get; init; }

    [Range(minimum: 0, maximum: int.MaxValue)]
    public decimal BetAmount { get; init; }
    [Range(minimum: 0, maximum: int.MaxValue)]
    public decimal ValidBetAmount { get; init; }
    public decimal SettleAmount { get; init; }
    [Range(minimum: 0, maximum: int.MaxValue)]
    public decimal Commission { get; init; }
    [Range(minimum: 0, maximum: int.MaxValue)]
    public decimal Turnover { get; init; }
    public decimal PlayerWinLoss { get; init; }
    public string AdditionalData { get; init; } = default!;
}
