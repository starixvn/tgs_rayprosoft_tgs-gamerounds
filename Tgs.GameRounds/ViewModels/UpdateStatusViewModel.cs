using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.ViewModels
{
    public record UpdateStatusViewModel(GameRoundStatus Status, DateTimeOffset? ExternalClosedOn);
}
