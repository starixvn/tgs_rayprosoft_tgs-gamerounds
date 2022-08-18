namespace Tgs.GameRounds.Core.ApplicationServices;

public record NewRoundQuery(
	  string ExternalRoundId,
	  Guid GameProviderId,
	  string PlayerCode,
	  int GameId,
	  string CurrencyCode,
	  Guid GameProviderConfigurationId,
	  Guid WalletId,
	  Guid BrandId,
	  DateTimeOffset ExternalCreatedOn);
