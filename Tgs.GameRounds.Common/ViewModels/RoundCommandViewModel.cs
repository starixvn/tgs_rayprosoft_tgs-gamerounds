using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace Tgs.GameRounds.Common.ViewModels;
public record RoundCommandViewModel
{
	[Required]
	public Guid Id { get; init; }
	[Required]
	public Guid GameProviderId { get; init; }
	[Required]
	public Guid GameProviderConfigurationId { get; init; }
	[Required]
	public int GameId { get; init; }
	[Required]
	public Guid BrandId { get; init; }
	[Required]
	public string PlayerCode { get; init; } = default!;
	[Required]
	public Guid WalletId { get; init; }
	[Required]
	public string CurrencyCode { get; init; } = default!;
	[Required]
	public Enums.RoundStatus Status { get; init; }
	[Required]
	public Enums.RoundClientStatus ClientStatus { get; init; }
	[Required]
	public string ExternalRoundId { get; init; } = default!;
	[Required]
	public DateTimeOffset ExternalCreatedOn { get; init; }
	public DateTimeOffset? ExternalClosedOn { get; init; }
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
	public decimal PlayerWinLoss { get; init; }
	[Required]
	public DateTimeOffset CreatedOn { get; init; }
}
