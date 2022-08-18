using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.Models;

public record UpdateRoundClosed(
	DateTimeOffset ExternalClosedOn,
	decimal BetAmount,
	decimal ValidBetAmount,
	decimal SettleAmount,
	decimal Commission,
	decimal Turnover,
	decimal PlayerWinLoss,
	GameRoundClientStatus ClientStatus,
	string AdditionalData);
