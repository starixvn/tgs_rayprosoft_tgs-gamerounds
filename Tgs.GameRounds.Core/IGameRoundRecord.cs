using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core;

public interface IGameRoundRecord
{
	public ValueTask AddAsync(
		AddGameRoundRecord add,
		CancellationToken cancellationToken = default);
}
