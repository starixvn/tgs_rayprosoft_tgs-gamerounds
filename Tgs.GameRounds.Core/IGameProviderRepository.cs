namespace Tgs.GameRounds.Core;

public interface IGameProviderRepository
{
    ValueTask<string> GetCode(
        Guid id,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<Guid> GetIds(
        CancellationToken cancellationToken = default);
}
