using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client
{
    public interface IGameRoundsClient
    {
        ValueTask<GameRoundResponse> AddRoundAsync(AddRoundViewModel model);

        ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid rounid);

        ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, string externalGameId, Guid walletId);

        ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<GameRoundRoundStatusResponse> GetStatusAsync(Guid rounid);

        ValueTask<GameRoundRoundIdResponse> NewRoundAsync(NewRoundQueryViewModel model);

        ValueTask<GameRoundResponse> UpdateClientStatusAsync(Guid roundId, GameRoundClientStatus status);

        ValueTask<bool> UpdateRoundAsync(Guid roundId, UpdateRoundViewModel model);

        ValueTask<GameRoundResponse> UpdateRoundClosedAsync(Guid roundId, UpdateRoundClosedViewModel model);

        ValueTask<GameRoundResponse> UpdateRoundDetailAsync(Guid roundId, string additionalData);

        ValueTask<GameRoundResponse> UpdateRoundOpenAsync(Guid roundId, UpdateRoundOpenViewModel model);

        ValueTask<GameRoundResponse> UpdateRoundOpenSettleAsync(Guid roundId, UpdateRoundOpenSettleViewModel model);

        ValueTask<GameRoundResponse> UpdateRoundReopenAsync(Guid roundId, UpdateRoundReopenViewModel model);

        ValueTask<GameRoundResponse> UpdateStatusAsync(Guid roundId, GameRoundStatus status, DateTimeOffset? externalClosedOn = null);
    }
}
