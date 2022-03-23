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

        ValueTask<NewRoundResultViewModel?> NewRoundAsync(NewRoundQueryViewModel model);

        ValueTask<UpdateRoundResultViewModel?> UpdateRoundAsync(Guid roundId, UpdateRoundQueryViewModel model);

        ValueTask UpdateRoundClosedAsync(Guid roundId, UpdateRoundClosedViewModel model);

        ValueTask UpdateRoundOpenAsync(Guid roundId, UpdateRoundOpenViewModel model);

        ValueTask UpdateRoundReopenAsync(Guid roundId, UpdateRoundReopenViewModel model);
    }
}
