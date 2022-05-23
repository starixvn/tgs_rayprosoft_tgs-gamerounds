using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client
{
    public interface IGameRoundsClient
    {
        ValueTask<GameRoundResponse> AddRoundAsync(AddRoundViewModel model);

        ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid roundId);

        ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, string externalGameId, Guid walletId);

        ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<GameRoundDetailDataResponse> GetRoundDetailAsync(Guid roundId);

        ValueTask<GameRoundRoundStatusResponse> GetStatusAsync(Guid roundId);

        ValueTask<GameRoundRoundIdResponse> NewRoundAsync(NewRoundQueryViewModel model);

        ValueTask<GameRoundResponse> UpdateClientStatusAsync(Guid roundId, GameRoundClientStatus status);

        ValueTask<GameRoundResponse> UpdateRoundClosedAsync(Guid roundId, UpdateRoundClosedViewModel model);

        ValueTask<GameRoundResponse> UpdateRoundDetailAsync(Guid roundId, string additionalData);

        ValueTask<GameRoundResponse> UpdateRoundOpenAsync(Guid roundId, UpdateRoundOpenViewModel model);

        ValueTask<GameRoundResponse> UpdateRoundOpenSettleAsync(Guid roundId, UpdateRoundOpenSettleViewModel model);

        ValueTask<GameRoundResponse> UpdateRoundReopenAsync(Guid roundId, UpdateRoundReopenViewModel model);

        ValueTask<GameRoundResponse> UpdateStatusAsync(Guid roundId, GameRoundStatus status, DateTimeOffset? externalClosedOn = null);
    }
}
