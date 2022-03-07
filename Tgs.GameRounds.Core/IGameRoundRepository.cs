using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core
{
    public interface IGameRoundRepository
    {
        ValueTask<GameRoundInfo> GetAsync(Guid roundId);

        ValueTask<GameRoundInfo?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId);

        ValueTask<Guid?> NewRoundAsync(NewRoundQuery round);

        ValueTask UpdateRoundAsync(UpdateRoundQuery round);
    }
}
