using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client
{
    public interface IGameRoundsClient
    {
        ValueTask AddRoundAsync(AddRoundViewModel model);

        ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid rounid);

        ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, string externalGameId, Guid walletId);

        ValueTask<GameRoundViewModel?> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<GameRoundStatus?> GetStatusAsync(Guid rounid);

        ValueTask<Guid?> NewRoundAsync(NewRoundQueryViewModel model);

        ValueTask UpdateClientStatusAsync(Guid roundId, GameRoundClientStatus status);

        ValueTask<bool> UpdateRoundAsync(Guid roundId, UpdateRoundViewModel model);

        ValueTask UpdateRoundClosedAsync(Guid roundId, UpdateRoundClosedViewModel model);

        ValueTask UpdateRoundDetailAsync(Guid roundId, string additionalData);

        ValueTask UpdateRoundOpenAsync(Guid roundId, UpdateRoundOpenViewModel model);

        ValueTask UpdateRoundReopenAsync(Guid roundId, UpdateRoundReopenViewModel model);

        ValueTask UpdateStatusAsync(Guid roundId, GameRoundStatus status, DateTimeOffset? externalClosedOn = null);

        ValueTask UpdateRoundOpenSettleAsync(Guid roundId, UpdateRoundOpenSettleViewModel model);
    }
}
