using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

public interface IGameRoundsWorkClient
{
	public ValueTask ExecuteAsync(GameRoundsWorkViewModel model, CancellationToken cancellationToken = default);
}
