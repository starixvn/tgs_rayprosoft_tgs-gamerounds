using MediatR;

namespace Tgs.GameRounds.Core.ApplicationServices;
public record RoundCommandRequest(
	Guid Id,
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
	DateTimeOffset CreatedOn) : IRequest<Guid>;
