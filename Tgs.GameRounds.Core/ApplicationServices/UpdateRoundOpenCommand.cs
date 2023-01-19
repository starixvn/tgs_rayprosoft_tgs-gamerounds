namespace Tgs.GameRounds.Core.ApplicationServices;

public record UpdateRoundOpenCommand(
    Guid RoundId,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal Commission,
    decimal Turnover);
