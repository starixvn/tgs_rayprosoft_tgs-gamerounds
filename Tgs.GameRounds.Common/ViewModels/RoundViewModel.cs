using Tgs.GameRounds.Common.Enums;

namespace Tgs.GameRounds.Common.ViewModels;

public record RoundViewModel
{
	public Guid RoundId { get; init; }
	public Guid GameProviderId { get; init; }
	public string GameProviderCode { get; init; } = default!;
	public Guid GameProviderConfigurationId { get; init; }
	public int GameId { get; init; }
	public Guid BrandId { get; init; }
	public string PlayerCode { get; init; } = default!;
	public Guid WalletId { get; init; }
	public string CurrencyCode { get; init; } = default!;
	public RoundStatus Status { get; init; }
	public RoundClientStatus ClientStatus { get; init; }
	public string ExternalRoundId { get; init; } = default!;
	public string ExternalGameId { get; init; } = default!;
	public DateTimeOffset? ExternalCreatedOn { get; init; }
	public DateTimeOffset? ExternalClosedOn { get; init; }
	public decimal BetAmount { get; init; }
	public decimal ValidBetAmount { get; init; }
	public decimal SettleAmount { get; init; }
	public decimal Commission { get; init; }
	public decimal Turnover { get; init; }
	public decimal PlayerWinLoss { get; init; }
	public decimal CompanyWinLoss { get; init; }
	public DateTimeOffset CreatedOn { get; init; }
	public DateTimeOffset? ClosedOn { get; init; }
	public DateTimeOffset UpdatedOn { get; init; }
	public string AdditionalData { get; init; } = default!;
}
