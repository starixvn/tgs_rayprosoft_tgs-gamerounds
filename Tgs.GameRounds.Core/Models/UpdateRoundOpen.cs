namespace Tgs.GameRounds.Core.Models;

public record UpdateRoundOpen(
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal Commission,
    decimal Turnover);
