namespace Tgs.GameRounds.Core.Models;

public record UpdateRoundOpenSettle(
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal SettleAmount,
    decimal Commission,
    decimal Turnover,
    decimal PlayerWinLoss);
