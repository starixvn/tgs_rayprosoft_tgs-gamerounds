using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.HandleServices;

internal class GameRoundCheckingHandleService : IGetService<GameRoundCheckingQuery, GameRoundCheckingStatus>
{
    private readonly ILogger<GameRoundCheckingHandleService> _logger;
    private readonly IGameRoundRepository _repository;

    public GameRoundCheckingHandleService(
        ILogger<GameRoundCheckingHandleService> logger,
        IGameRoundRepository repository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async ValueTask<GameRoundCheckingStatus> GetAsync(
        GameRoundCheckingQuery query,
        CancellationToken cancellationToken = default)
    {
        var id = await _repository
            .GetRoundIdAsync(
                gameProviderId: query.GameProviderId,
                externalRoundId: query.ExternalRoundId,
                gameId: query.GameId,
                walletId: query.WalletId,
                externalCreatedOn: query.ExternalCreatedOn,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!id.HasValue)
        {
            return GameRoundCheckingStatus.Change;
        }
        else
        {
            _logger.LogInformation("This game round information: game round ID({id})-game provider ID({gameProviderId})-extension ID({externalRoundId})-game ID({gameId})-player({playerCode})-wallet ID({walletId})",
                id,
                query.GameProviderId,
                query.ExternalRoundId,
                query.GameId,
                query.PlayerCode,
                query.WalletId);

            var info = await _repository
                .GetAsync(
                    id: id.Value,
                    externalCreatedOn: query.ExternalCreatedOn,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new Exception($"This game round order number can not find data.({id})");

            if (info.Status == GameRoundStatus.Open && query.Status == GameRoundStatus.Open)
            {
                if (info.BetAmount != query.BetAmount ||
                    info.ValidBetAmount != query.ValidBetAmount)
                {
                    return GameRoundCheckingStatus.Change;
                }
            }
            else
            {
                if (info.Status == GameRoundStatus.Open && query.Status == GameRoundStatus.Closed)
                {
                    return GameRoundCheckingStatus.Change;
                }
                else
                {
                    if (info.Status == GameRoundStatus.Closed && query.Status == GameRoundStatus.Closed)
                    {
                        if (info.BetAmount != query.BetAmount ||
                        info.ValidBetAmount != query.ValidBetAmount ||
                        info.SettleAmount != query.SettleAmount ||
                        info.Commission != query.Commission ||
                        info.Turnover != query.Turnover ||
                        info.CompanyWinLoss != -query.PlayerWinLoss ||
                        info.ClientStatus != query.ClientStatus)
                        {
                            return GameRoundCheckingStatus.Change;
                        }
                    }
                }
            }
        }

        return GameRoundCheckingStatus.Unchanged;
    }
}
