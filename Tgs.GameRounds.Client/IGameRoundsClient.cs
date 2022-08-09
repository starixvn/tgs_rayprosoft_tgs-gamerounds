using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client
{
    public interface IGameRoundsClient
    {
        ValueTask<GameRoundResponse> AddRoundAsync(AddRoundViewModel model, CancellationToken cancellationToken = default);

        ValueTask<GameRoundBatchResponse> AddRoundBatchAsync(AddRoundBatchsViewModel model, CancellationToken cancellationToken = default);

        ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid roundId, CancellationToken cancellationToken = default);

        ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, string externalGameId, Guid walletId, CancellationToken cancellationToken = default);

        ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default);

        ValueTask<GameRoundDetailDataResponse> GetRoundDetailAsync(Guid roundId, CancellationToken cancellationToken = default);

        ValueTask<GameRoundIdResponse> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default);

        ValueTask<GameRoundRoundStatusResponse> GetStatusAsync(Guid roundId, CancellationToken cancellationToken = default);

        ValueTask<GameRoundRoundIdResponse> NewRoundAsync(NewRoundQueryViewModel model, CancellationToken cancellationToken = default);

        ValueTask<GameRoundResponse> UpdateClientStatusAsync(Guid roundId, GameRoundClientStatus status, CancellationToken cancellationToken = default);

        ValueTask<GameRoundResponse> UpdateRoundClosedAsync(Guid roundId, UpdateRoundClosedViewModel model, CancellationToken cancellationToken = default);

        ValueTask<GameRoundResponse> UpdateRoundDetailAsync(Guid roundId, string additionalData, CancellationToken cancellationToken = default);

        ValueTask<GameRoundResponse> UpdateRoundOpenAsync(Guid roundId, UpdateRoundOpenViewModel model, CancellationToken cancellationToken = default);

        ValueTask<GameRoundResponse> UpdateRoundOpenSettleAsync(Guid roundId, UpdateRoundOpenSettleViewModel model, CancellationToken cancellationToken = default);

        ValueTask<GameRoundResponse> UpdateRoundReopenAsync(Guid roundId, UpdateRoundReopenViewModel model, CancellationToken cancellationToken = default);

        ValueTask<GameRoundResponse> UpdateStatusAsync(Guid roundId, GameRoundStatus status, DateTimeOffset? externalClosedOn = null, CancellationToken cancellationToken = default);
    }
}
