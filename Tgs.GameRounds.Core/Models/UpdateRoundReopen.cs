using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.Models
{
    public record UpdateRoundReopen(
        Guid RoundId,
        GameRoundStatus Status,
        DateTimeOffset? ExternalClosedOn,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal SettleAmount,
        decimal Commission,
        decimal Turnover,
        decimal WinLoss,
        GameRoundClientStatus ClientStatus,
        string AdditionalData);
}
