using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

public interface IFailedGameRoundsClient
{
    ValueTask AddAsync(
        AddFailedGameRoundViewModel source,
        CancellationToken cancellationToken = default);

    ValueTask<FailedGameRoundInfoViewModel?> GetAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        string playerCode,
        CancellationToken cancellationToken = default);
}
