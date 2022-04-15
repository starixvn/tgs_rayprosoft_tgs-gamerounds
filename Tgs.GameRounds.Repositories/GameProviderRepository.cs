using Tgs.GameRounds.Core;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories
{
    internal class GameProviderRepository : IGameProvider
    {
        private readonly TgsContext _context;
#pragma warning disable IDE0052 // 刪除未讀取的私用成員
        private readonly IGameProvider _gameProvider;
#pragma warning restore IDE0052 // 刪除未讀取的私用成員

        public GameProviderRepository(
            IGameProvider gameProvider,
            TgsContext context)
        {
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ValueTask<string> GetGameProviderCodeAsync(Guid gameProviderId)
            => _context.GameProviders.AsQueryable()
                .Where(x => x.Id == gameProviderId)
                .Select(x => x.Code.Trim())
                .ToAsyncEnumerable()
                .SingleAsync();
    }
}
