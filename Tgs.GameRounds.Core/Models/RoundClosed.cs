namespace Tgs.GameRounds.Core.Models;
public record RoundClosed(
	Common.Enums.RoundClientStatus ClientStatus,
	DateTimeOffset ExternalClosedOn,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal PlayerWinLoss);
