using Microsoft.EntityFrameworkCore;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories
{
    internal class GameRepository : IGameRepository
    {
        private readonly TgsContext _context;

        public GameRepository(
            TgsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ValueTask<int?> GetGameIdAsync(Guid gameProviderId, string externalGameId)
            => _context.Games.AsQueryable()
            .Where(x => x.GameProviderId == gameProviderId && x.ExternalId == externalGameId)
            .Select(x => (int?)x.Id)
            .ToAsyncEnumerable()
            .FirstOrDefaultAsync();
    }
}
