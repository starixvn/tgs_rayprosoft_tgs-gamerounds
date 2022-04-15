using Microsoft.EntityFrameworkCore;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories
{
    internal class GameRepository : IGame
    {
        private readonly TgsContext _context;
#pragma warning disable IDE0052 // 刪除未讀取的私用成員
        private readonly IGame _game;
#pragma warning restore IDE0052 // 刪除未讀取的私用成員

        public GameRepository(
            IGame game,
            TgsContext context)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ValueTask<string> GetExternalGameIdAsync(int gameId)
            => _context.Games.AsQueryable()
                .Where(x => x.Id == gameId)
                .Select(x => x.ExternalId.Trim())
                .ToAsyncEnumerable()
                .SingleAsync();

        public ValueTask<int> GetGameIdAsync(Guid gameProviderId, string externalGameId)
            => _context.Games.AsQueryable()
                .Where(x => x.GameProviderId == gameProviderId && x.ExternalId == externalGameId)
                .Select(x => x.Id)
                .ToAsyncEnumerable()
                .SingleAsync();
    }
}
