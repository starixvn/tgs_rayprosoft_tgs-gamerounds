using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core;

public interface IGameRoundConvert
{
    ValueTask<GameRoundConvertResult> ConvertPlatformRoundsAsync(
        GameRoundConvertRequest request,
        CancellationToken cancellationToken = default);
}
