using Tgs.GameRounds.Common.Enums;

namespace Tgs.GameRounds.Core.ApplicationServices;

public record RoundInfoResponse(
	Guid RoundId,
	Guid GameProviderId,
	string GameProviderCode,
	Guid GameProviderConfigurationId,
	int GameId,
	Guid BrandId,
	string PlayerCode,
	Guid WalletId,
	string CurrencyCode,
	RoundStatus Status,
	RoundClientStatus ClientStatus,
	string ExternalRoundId,
	string ExternalGameId,
	DateTimeOffset? ExternalCreatedOn,
	DateTimeOffset? ExternalClosedOn,
	decimal BetAmount,
	decimal ValidBetAmount,
	decimal SettleAmount,
	decimal Commission,
	decimal Turnover,
	decimal PlayerWinLoss,
	DateTimeOffset CreatedOn,
	DateTimeOffset? ClosedOn,
	DateTimeOffset UpdatedOn,
	string AdditionalData);
