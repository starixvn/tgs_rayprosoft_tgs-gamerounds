using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core;

public interface IGameRoundRepository
{
    ValueTask<Guid?> AddCompanyRoundAsync(
        AddCompanyRound source,
        CancellationToken cancellationToken = default);

    ValueTask AdjustClosedRoundAsync(
        Guid id,
        AdjustClosedRound source,
        CancellationToken cancellationToken = default);

    ValueTask<bool> ExistAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    ValueTask<GameRoundInfo?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    ValueTask<GameRoundInfo?> GetAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        CancellationToken cancellationToken = default);

    ValueTask<GameRoundDetailInfo?> GetRoundDetailAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    ValueTask<Guid?> GetRoundIdAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        CancellationToken cancellationToken = default);

    ValueTask<GameRoundStatus> GetStatusAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    ValueTask ManualCloseRoundAsync(
        Guid id,
        ManualCloseRound source,
        CancellationToken cancellationToken = default);
}
