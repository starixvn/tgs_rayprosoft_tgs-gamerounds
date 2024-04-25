using System.Text.Json.Nodes;

namespace Tgs.GameRounds.Core.ApplicationServices;
public record RoundCommand(
	Guid GameProviderId,
	string ExternalRoundId,
	int GameId,
	string PlayerCode,
	Guid WalletId,
	string CurrencyCode,
	Guid BrandId,
	Guid GameProviderConfigurationId,
	Common.Enums.RoundStatus Status,
	Common.Enums.RoundClientStatus ClientStatus,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset? ExternalClosedOn,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal PlayerWinLoss,
	JsonNode AdditionalData);
