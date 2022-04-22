using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    public record UpdateRoundClosedCommand(
        Guid RoundId,
        DateTimeOffset ExternalClosedOn,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal SettleAmount,
        decimal Commission,
        decimal Turnover,
        decimal PlayerWinLoss,
        GameRoundClientStatus ClientStatus,
        string AdditionalData);
}
