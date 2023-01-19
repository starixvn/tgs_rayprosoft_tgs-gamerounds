using System.Runtime.CompilerServices;

using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class GetListByRoundIdQueryService : IQueryService<GetListByRoundIdQuery, Guid>
{
    private readonly IGame _game;
    private readonly IGameRound _gameRound;

    public GetListByRoundIdQueryService(
        IGame game,
        IGameRound gameRound)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
    }

    public async IAsyncEnumerable<Guid> QueryAsync(GetListByRoundIdQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var gameId = await _game
                    .GetGameIdAsync(
                        gameProviderId: query.GameProviderId,
                        externalGameId: query.ExternalGameId,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

        var roundIds = await _gameRound
                            .GetListByRoundIdAsync(
                                query.GameProviderId,
                                query.ExternalRoundId,
                                gameId)
                            .ToArrayAsync(
                                cancellationToken: cancellationToken)
                            .ConfigureAwait(false);

        if (!roundIds.Any())
        {
            yield break;
        }

        foreach (var roundId in roundIds)
        {
            yield return roundId;
        }
    }
}
