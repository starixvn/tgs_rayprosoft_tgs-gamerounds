using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

public interface IGameRoundsClient
{
    ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid roundId, CancellationToken cancellationToken = default);

    ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default);

    ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, string externalGameId, Guid walletId, CancellationToken cancellationToken = default);

    ValueTask<GameRoundDetailViewModel?> GetRoundDetailAsync(Guid roundId, CancellationToken cancellationToken = default);
}
