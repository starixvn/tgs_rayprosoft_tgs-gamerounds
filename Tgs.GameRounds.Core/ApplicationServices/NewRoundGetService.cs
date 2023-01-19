using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class NewRoundGetService : IGetService<NewRoundQuery, Guid>
{
    private readonly IGameRound _gameRound;
    private readonly IGameRoundLocked _gameRoundLocked;
    private readonly IGameRoundRecordMessage _gameRoundRecordMessage;
    private readonly ILogger<NewRoundGetService> _logger;

    public NewRoundGetService(
        ILogger<NewRoundGetService> logger,
        IGameRound gameRound,
        IGameRoundLocked gameRoundLocked,
        IGameRoundRecordMessage gameRoundRecordMessage)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
        _gameRoundLocked = gameRoundLocked ?? throw new ArgumentNullException(nameof(gameRoundLocked));
        _gameRoundRecordMessage = gameRoundRecordMessage ?? throw new ArgumentNullException(nameof(gameRoundRecordMessage));
    }

    public async ValueTask<Guid> GetAsync(NewRoundQuery query, CancellationToken cancellationToken = default)
    {
        var roundId = await _gameRound.GetRoundIdAsync(
                gameProviderId: query.GameProviderId,
                externalRoundId: query.ExternalRoundId,
                gameId: query.GameId,
                walletId: query.WalletId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

        if (roundId.HasValue)
        {
            return roundId.Value;
        }

        if (!await _gameRoundLocked.LockingAsync(
                gameProviderId: query.GameProviderId,
                externalRoundId: query.ExternalRoundId,
                gameId: query.GameId,
                walletId: query.WalletId,
                cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            roundId = await _gameRound.GetRoundIdAsync(
                gameProviderId: query.GameProviderId,
                externalRoundId: query.ExternalRoundId,
                gameId: query.GameId,
                walletId: query.WalletId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            return roundId ?? throw new GameRoundCreationFailedException();
        }

        try
        {
            var newRound = new NewRound(
                    query.ExternalRoundId,
                    query.GameProviderId,
                    query.PlayerCode,
                    query.GameId,
                    query.CurrencyCode,
                    query.GameProviderConfigurationId,
                    query.WalletId,
                    query.BrandId,
                    query.ExternalCreatedOn);

            var newRoundId = await _gameRound.NewRoundAsync(
                source: newRound,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!newRoundId.HasValue)
            {
                _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundCreationFailedException", DateTime.Now.ToString("o"), query);
                throw new GameRoundCreationFailedException();
            }
            else
            {
                await _gameRoundRecordMessage.SenderGameRoundRecord(
                    roundId: newRoundId.Value,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            return newRoundId.Value;
        }
        finally
        {
            await _gameRoundLocked.UnlockAsync(
                    gameProviderId: query.GameProviderId,
                    externalRoundId: query.ExternalRoundId,
                    gameId: query.GameId,
                    walletId: query.WalletId,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
