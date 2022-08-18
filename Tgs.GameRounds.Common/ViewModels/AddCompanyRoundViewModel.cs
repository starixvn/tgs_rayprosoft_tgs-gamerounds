using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRounds.Common.ViewModels;

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
