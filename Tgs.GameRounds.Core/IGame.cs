namespace Tgs.GameRounds.Core;

public interface IGame
{
    public ValueTask<string> GetExternalGameIdAsync(
        int gameId,
        CancellationToken cancellationToken = default);

    public ValueTask<int> GetGameIdAsync(
        Guid gameProviderId,
        string externalGameId,
        CancellationToken cancellationToken = default);
}
