using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    public record UpdateStatusCommand(Guid RoundId, GameRoundStatus Status, DateTimeOffset? ExternalClosedOn = null);
}
