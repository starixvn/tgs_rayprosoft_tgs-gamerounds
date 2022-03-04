using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class NewRoundGetService : IGetService<NewRoundQuery, NewRoundResult>
    {
        private readonly IGameRoundCacheService _gameRoundCacheService;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly IGameRoundRepository _gameRoundRepository;

        public NewRoundGetService(
          IGameRoundCacheService gameRoundCacheService,
          IGameRoundRecordRepository gameRoundRecordRepository,
          IGameRoundRepository gameRoundRepository)
        {
            _gameRoundCacheService = gameRoundCacheService ?? throw new ArgumentNullException(nameof(gameRoundCacheService));
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
        }

        public async ValueTask<NewRoundResult> GetAsync(NewRoundQuery query, CancellationToken cancellationToken = default)
        {
            var roundId = await _gameRoundCacheService.GetRoundIdAsync(query.GameProviderId, query.ExternalRoundId, query.GameId, query.WalletId).ConfigureAwait(false);
            if (roundId.HasValue)
            {
                return new(Success: true, RoundId: roundId.Value);
            }

            var newRoundId = await _gameRoundRepository.NewRoundAsync(query).ConfigureAwait(false);
            if (!newRoundId.HasValue)
            {
                return new();
            }

            var roundRecord = await _gameRoundRepository.GetAsync(newRoundId.Value).ConfigureAwait(false);

            await _gameRoundRecordRepository.AddAsync(new(
                roundRecord.RoundId,
                roundRecord.ExternalRoundId,
                roundRecord.GameProviderId,
                roundRecord.PlayerCode,
                roundRecord.GameId,
                roundRecord.CurrencyCode,
                roundRecord.Status,
                roundRecord.GameProviderConfigurationId,
                roundRecord.WalletId,
                roundRecord.CreatedOn,
                roundRecord.ClosedOn,
                roundRecord.UpdatedOn,
                roundRecord.BrandId,
                roundRecord.ExternalCreatedOn,
                roundRecord.ExternalClosedOn,
                roundRecord.BetAmount,
                roundRecord.ValidBetAmount,
                roundRecord.SettleAmount,
                roundRecord.Commission,
                roundRecord.Turnover,
                roundRecord.WinLoss,
                roundRecord.TieBetAdjustment,
                roundRecord.ClientStatus,
                roundRecord.ReopenTurnoverAdjustment,
                roundRecord.ReopenedOn,
                roundRecord.EffectiveBettingPercentage,
                roundRecord.AdditionalData)).ConfigureAwait(false);

            return new NewRoundResult(Success: true, RoundId: newRoundId);
        }
    }
}
