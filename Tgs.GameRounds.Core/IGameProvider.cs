namespace Tgs.GameRounds.Core
{
    public interface IGameProvider
    {
        ValueTask<string> GetGameProviderCodeAsync(
            Guid gameProviderId,
            CancellationToken cancellationToken = default);
    }
}
