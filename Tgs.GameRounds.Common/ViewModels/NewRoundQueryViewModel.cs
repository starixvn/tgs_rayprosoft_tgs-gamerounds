using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRounds.Common.ViewModels;

public record NewRoundQueryViewModel
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
}
