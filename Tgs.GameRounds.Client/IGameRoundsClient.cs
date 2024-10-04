using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

public interface IGameRoundsClient
{
	ValueTask<Guid> RoundCommandAsync(RoundCommandViewModel source, CancellationToken cancellationToken = default);
}
