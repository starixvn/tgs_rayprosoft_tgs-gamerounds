namespace Tgs.GameRounds.Core.Models;
public record RoundReopen(
	Common.Enums.RoundClientStatus ClientStatus,
	DateTimeOffset ExternalClosedOn,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal PlayerWinLoss);
