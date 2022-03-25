namespace Tgs.GameRounds.Core
{
    internal class NoopGameProvider : IGameProvider
    {
        public ValueTask<string> GetGameProviderCodeAsync(Guid gameProviderId) => throw new NotSupportedException();
    }
}
