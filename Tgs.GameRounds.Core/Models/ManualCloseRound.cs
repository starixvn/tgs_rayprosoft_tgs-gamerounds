using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.Models;

public record ManualCloseRound(
    DateTimeOffset ExternalClosedOn,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal SettleAmount,
    decimal Commission,
    decimal Turnover,
    decimal CompanyWinLoss,
    GameRoundClientStatus ClientStatus,
    Guid OperatorId,
    string Remark);
