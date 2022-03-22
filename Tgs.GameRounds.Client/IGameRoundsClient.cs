using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client
{
    internal interface IGameRoundsClient
    {
        ValueTask AddRoundAsync(AddRoundViewModel model);

        ValueTask<GameRoundViewModel?> GetGameRound(Guid rounid);

        ValueTask<GameRoundViewModel?> GetGameRound(Guid gameProviderId, string externalRoundId, string externalGameId, Guid walletId);

        ValueTask<GameRoundViewModel?> GetGameRound(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<GameRoundStatus?> GetStatus(Guid rounid);

        ValueTask<NewRoundResultViewModel?> NewRoundAsync(NewRoundQueryViewModel model);

        ValueTask<UpdateRoundResultViewModel?> UpdateRoundAsync(Guid roundId, UpdateRoundQueryViewModel model);

        ValueTask UpdateRoundClosedAsync(Guid roundId, UpdateRoundClosedViewModel model);

        ValueTask UpdateRoundOpenAsync(Guid roundId, UpdateRoundOpenViewModel model);

        ValueTask UpdateRoundReopenAsync(Guid roundId, UpdateRoundReopenViewModel model);
    }
}
