namespace Tgs.GameRounds.Core;

public interface IGameRepository
{
	public ValueTask<string> GetExternalIdAsync(int id, CancellationToken cancellationToken = default);

	public ValueTask<int> GetIdAsync(Guid gameProviderId, string externalId, CancellationToken cancellationToken = default);
}
