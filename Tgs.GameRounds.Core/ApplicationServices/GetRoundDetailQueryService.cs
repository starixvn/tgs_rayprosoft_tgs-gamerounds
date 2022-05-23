using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class GetRoundDetailQueryService : IGetService<GetRoundDetailQuery, GameRoundDetailResult?>
    {
        private readonly IGameRound _gameRound;
        private readonly ILogger<GetRoundDetailQueryService> _logger;

        public GetRoundDetailQueryService(
            IGameRound gameRound,
            ILogger<GetRoundDetailQueryService> logger)
        {
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<GameRoundDetailResult?> GetAsync(GetRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var gameRoundDetailInfo = await _gameRound
                .GetRoundDetailAsync(query.RoundId)
                .ConfigureAwait(false);

            if (gameRoundDetailInfo is not null)
            {
                return new GameRoundDetailResult(
                    gameRoundDetailInfo.RoundId,
                    gameRoundDetailInfo.AdditionalData);
            }

            return default;
        }
    }
}
