using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    public record UpdateRoundClosedCommand(
        Guid RoundId,
        DateTimeOffset? ClosedOn,
        DateTimeOffset UpdatedOn,
        DateTimeOffset? ExternalClosedOn,
        decimal? BetAmount,
        decimal? ValidBetAmount,
        decimal? SettleAmount,
        decimal? Commission,
        decimal? Turnover,
        decimal? WinLoss,
        decimal? TieBetAdjustment,
        GameRoundClientStatus ClientStatus,
        string AdditionalData);
}
