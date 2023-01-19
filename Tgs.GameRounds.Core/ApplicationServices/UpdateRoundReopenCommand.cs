using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.ApplicationServices;

public record UpdateRoundReopenCommand(
    Guid RoundId,
    GameRoundStatus Status,
    DateTimeOffset? ExternalClosedOn,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal SettleAmount,
    decimal Commission,
    decimal Turnover,
    decimal PlayerWinLoss,
    GameRoundClientStatus ClientStatus,
    string AdditionalData);
