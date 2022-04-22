using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class NewRoundGetService : IGetService<NewRoundQuery, Guid>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly ILogger<NewRoundGetService> _logger;

        public NewRoundGetService(
            IGameRoundRecordRepository gameRoundRecordRepository,
            IGameRound gameRound,
            ILogger<NewRoundGetService> logger)
        {
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<Guid> GetAsync(NewRoundQuery query, CancellationToken cancellationToken = default)
        {
            var roundId = await _gameRound.GetRoundIdAsync(query.GameProviderId, query.ExternalRoundId, query.GameId, query.WalletId).ConfigureAwait(false);
            if (roundId.HasValue)
            {
                return roundId.Value;
            }

            var newRoundId = await _gameRound.NewRoundAsync(
                new NewRound(
                    query.ExternalRoundId,
                    query.GameProviderId,
                    query.PlayerCode,
                    query.GameId,
                    query.CurrencyCode,
                    query.GameProviderConfigurationId,
                    query.WalletId,
                    query.BrandId,
                    query.ExternalCreatedOn)).ConfigureAwait(false);

            if (!newRoundId.HasValue)
            {
                _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundCreationFailedException", DateTime.Now.ToString("o"), query);
                throw new GameRoundCreationFailedException();
            }
            else
            {
                var round = await _gameRound.GetAsync(newRoundId.Value).ConfigureAwait(false);

                if (round is not null)
                {
                    await _gameRoundRecordRepository.AddAsync(round).ConfigureAwait(false);
                }
            }

            return newRoundId.Value;
        }
    }
}
