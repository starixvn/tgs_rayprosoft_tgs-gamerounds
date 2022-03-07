using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client
{
    internal interface IGameRoundsClient
    {
        ValueTask<GameRoundStatus?> GameRoundGetStatus(Guid rounid);

        ValueTask<NewRoundResultViewModel?> GameRoundsNewRoundAsync(NewRoundQueryViewModel model);

        ValueTask<UpdateRoundResultViewModel?> GameRoundsUpdateRoundAsync(Guid roundId, UpdateRoundQueryViewModel model);
    }
}
