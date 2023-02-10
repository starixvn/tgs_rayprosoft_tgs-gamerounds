using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

public interface IGameRoundsCheckingClient
{
    public ValueTask<GameRoundCheckingStatus> GetCheckingStatusAsync(GameRoundCheckingViewModel source, CancellationToken cancellationToken = default);
}
