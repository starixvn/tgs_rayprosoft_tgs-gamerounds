using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    public record UpdateClientStatusCommand(Guid RoundId, GameRoundClientStatus Status);
}
