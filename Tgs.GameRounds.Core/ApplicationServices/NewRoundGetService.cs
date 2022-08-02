using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class NewRoundGetService : IGetService<NewRoundQuery, Guid>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundLocked _gameRoundLocked;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly ILogger<NewRoundGetService> _logger;

        public NewRoundGetService(
            ILogger<NewRoundGetService> logger,
            IGameRound gameRound,
            IGameRoundLocked gameRoundLocked,
            IGameRoundRecordRepository gameRoundRecordRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _gameRoundLocked = gameRoundLocked ?? throw new ArgumentNullException(nameof(gameRoundLocked));
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
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
                    var round = await _gameRound.GetAsync(
                        roundId: newRoundId.Value,
                        cancellationToken: cancellationToken).ConfigureAwait(false);

                    if (round is not null)
                    {
                        await _gameRoundRecordRepository.AddAsync(
                            gameRound: round,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
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
}
