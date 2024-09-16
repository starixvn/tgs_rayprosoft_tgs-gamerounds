using System.Text.Json.Nodes;

namespace Tgs.GameRounds.Common.ViewModels;
public record RoundCommandViewModel
{
	public Guid? Id { get; init; }
	public Guid GameProviderId { get; init; }
	public Guid GameProviderConfigurationId { get; init; }
	public int GameId { get; init; }
	public Guid BrandId { get; init; }
	public string PlayerCode { get; init; } = default!;
	public Guid WalletId { get; init; }
	public string CurrencyCode { get; init; } = default!;
	public Enums.RoundStatus Status { get; init; }
	public Enums.RoundClientStatus ClientStatus { get; init; }
	public string ExternalRoundId { get; init; } = default!;
	public DateTimeOffset ExternalCreatedOn { get; init; }
	public DateTimeOffset? ExternalClosedOn { get; init; }
	public decimal BetAmount { get; init; }
	public decimal ValidBetAmount { get; init; }
	public decimal SettleAmount { get; init; }
	public decimal Commission { get; init; }
	public decimal PlayerWinLoss { get; init; }
	public DateTimeOffset? CreatedOn { get; init; }
	public JsonNode AdditionalData { get; init; } = default!;
}
