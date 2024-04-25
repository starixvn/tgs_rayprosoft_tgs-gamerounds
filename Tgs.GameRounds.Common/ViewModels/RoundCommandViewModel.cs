using System.Text.Json.Nodes;
using Tgs.GameRounds.Common.Enums;

namespace Tgs.GameRounds.Common.ViewModels;
public record RoundCommandViewModel(
	Guid GameProviderId,
	Guid GameProviderConfigurationId,
	int GameId,
	Guid BrandId,
	string PlayerCode,
	Guid WalletId,
	string CurrencyCode,
	RoundStatus Status,
	RoundClientStatus ClientStatus,
	string ExternalRoundId,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset? ExternalClosedOn,
	decimal BetAmount,
	decimal ValidBetAmount,
	decimal SettleAmount,
	decimal Commission,
	decimal Turnover,
	decimal PlayerWinLoss,
	JsonNode AdditionalData);
