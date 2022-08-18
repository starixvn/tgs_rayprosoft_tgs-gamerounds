namespace Tgs.GameRounds.Common.ViewModels;

public record AddRoundBatchViewModel(
	string ExternalRoundId,
	Guid GameProviderId,
	string PlayerCode,
	int GameId,
	string CurrencyCode,
	GameRoundStatus Status,
	Guid GameProviderConfigurationId,
	Guid WalletId,
	Guid BrandId,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset? ExternalClosedOn,
	decimal BetAmount,
	decimal ValidBetAmount,
	decimal? SettleAmount,
	decimal Commission,
	decimal Turnover,
	decimal? PlayerWinLoss,
	GameRoundClientStatus ClientStatus = GameRoundClientStatus.Open,
	string? AdditionalData = null);
