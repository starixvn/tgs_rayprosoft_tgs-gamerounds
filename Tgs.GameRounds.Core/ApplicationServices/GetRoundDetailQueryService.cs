using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class GetRoundDetailQueryService : IGetService<GetRoundDetailQuery, GameRoundDetailResult?>
    {
        private readonly IGameRound _gameRound;

        public GetRoundDetailQueryService(
            IGameRound gameRound)
        {
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
        }

        public async ValueTask<GameRoundDetailResult?> GetAsync(GetRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var gameRoundDetailInfo = await _gameRound
                .GetRoundDetailAsync(query.RoundId)
                .ConfigureAwait(false);

            return gameRoundDetailInfo is not null
                ? new GameRoundDetailResult(
                    gameRoundDetailInfo.RoundId,
                    gameRoundDetailInfo.AdditionalData)
                : default;
        }
    }
}
