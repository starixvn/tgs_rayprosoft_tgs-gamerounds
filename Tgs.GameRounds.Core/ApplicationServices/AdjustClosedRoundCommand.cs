using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    public record AdjustClosedRoundCommand(
        Guid RoundId,
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
}
