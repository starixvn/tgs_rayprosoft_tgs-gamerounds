using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core;

public interface IGameRoundRepository
{
    ValueTask<Guid?> AddCompanyRoundAsync(
        AddCompanyRound source,
        CancellationToken cancellationToken = default);

    ValueTask<Guid?> AddRoundAsync(
        AddRound source,
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

    ValueTask UpdateRoundClosedAsync(
        Guid id,
        UpdateRoundClosed source,
        CancellationToken cancellationToken = default);

    ValueTask UpdateRoundDetailAsync(
        Guid id,
        string additionalData,
        CancellationToken cancellationToken = default);

    ValueTask UpdateRoundOpenAsync(
        Guid id,
        UpdateRoundOpen source,
        CancellationToken cancellationToken = default);

    ValueTask UpdateRoundReopenAsync(
        Guid id,
        UpdateRoundReopen source,
        CancellationToken cancellationToken = default);
}
