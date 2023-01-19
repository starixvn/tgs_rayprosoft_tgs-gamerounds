using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

public interface IGameRoundsClient
{
    ValueTask<GameRoundResponse> AddCompanyRoundAsync(AddCompanyRoundViewModel model, CancellationToken cancellationToken = default);

    ValueTask<GameRoundResponse> AdjustClosedRoundAsync(Guid roundId, AdjustClosedRoundViewModel model, CancellationToken cancellationToken = default);

    ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid roundId, CancellationToken cancellationToken = default);

    ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, string externalGameId, Guid walletId, CancellationToken cancellationToken = default);

    ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default);

    IAsyncEnumerable<Guid> GetListByRoundIdAsync(Guid gameProviderId, string externalRoundId, string externalGameId, CancellationToken cancellationToken = default);

    ValueTask<GameRoundDetailDataResponse> GetRoundDetailAsync(Guid roundId, CancellationToken cancellationToken = default);

    ValueTask<GameRoundIdResponse> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default);

    ValueTask<GameRoundResponse> ManualCloseRoundAsync(Guid roundId, ManualCloseRoundViewModel model, CancellationToken cancellationToken = default);

    ValueTask<GameRoundResponse> UpdateRoundDetailAsync(Guid roundId, string additionalData, CancellationToken cancellationToken = default);
}
