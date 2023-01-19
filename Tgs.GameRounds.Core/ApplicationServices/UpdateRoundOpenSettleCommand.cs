namespace Tgs.GameRounds.Core.ApplicationServices;

public record UpdateRoundOpenSettleCommand(
    Guid RoundId,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal SettleAmount,
    decimal Commission,
    decimal Turnover,
    decimal PlayerWinLoss);
