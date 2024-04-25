namespace Tgs.GameRounds.Core;

public interface IGameProviderRepository
{
	ValueTask<string> GetCodeAsync(Guid id, CancellationToken cancellationToken = default);
}
