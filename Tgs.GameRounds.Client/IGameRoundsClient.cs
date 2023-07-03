using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

public interface IGameRoundsClient
{
    ValueTask AddCompanyRoundAsync(AddCompanyRoundViewModel source, CancellationToken cancellationToken = default);

    ValueTask AdjustClosedRoundAsync(Guid roundId, AdjustClosedRoundViewModel source, CancellationToken cancellationToken = default);

    ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid roundId, CancellationToken cancellationToken = default);

    ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default);

    ValueTask<GameRoundDetailViewModel?> GetRoundDetailAsync(Guid roundId, CancellationToken cancellationToken = default);

    ValueTask ManualCloseRoundAsync(Guid roundId, ManualCloseRoundViewModel source, CancellationToken cancellationToken = default);
}
