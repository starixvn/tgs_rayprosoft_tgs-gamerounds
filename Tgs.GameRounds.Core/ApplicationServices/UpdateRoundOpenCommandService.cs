using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateRoundOpenCommandService : ICommandService<UpdateRoundOpenCommand>
    {
        private readonly IGameRoundService _gameRoundCacheService;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly IGameRoundRepository _gameRoundRepository;

        public UpdateRoundOpenCommandService(
          IGameRoundService gameRoundCacheService,
          IGameRoundRecordRepository gameRoundRecordRepository,
          IGameRoundRepository gameRoundRepository)
        {
            _gameRoundCacheService = gameRoundCacheService ?? throw new ArgumentNullException(nameof(gameRoundCacheService));
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
        }

        public async ValueTask ExecuteAsync(UpdateRoundOpenCommand command, CancellationToken cancellationToken = default)
        {
            if (!await _gameRoundCacheService.ExistAsync(command.RoundId).ConfigureAwait(false))
            {
                return;
            }

            await _gameRoundRepository.UpdateRoundOpenAsync(new(
                command.RoundId,
                command.UpdatedOn,
                command.BetAmount,
                command.ValidBetAmount,
                command.Commission,
                command.Turnover,
                command.AdditionalData)).ConfigureAwait(false);

            var round = await _gameRoundRepository.GetAsync(command.RoundId).ConfigureAwait(false);

            await _gameRoundRecordRepository.AddAsync(new(
                round.RoundId,
                round.ExternalRoundId,
                round.GameProviderId,
                round.PlayerCode,
                round.GameId,
                round.CurrencyCode,
                round.Status,
                round.GameProviderConfigurationId,
                round.WalletId,
                round.CreatedOn,
                round.ClosedOn,
                round.UpdatedOn,
                round.BrandId,
                round.ExternalCreatedOn,
                round.ExternalClosedOn,
                round.BetAmount,
                round.ValidBetAmount,
                round.SettleAmount,
                round.Commission,
                round.Turnover,
                round.WinLoss,
                round.TieBetAdjustment,
                round.ClientStatus,
                round.ReopenTurnoverAdjustment,
                round.ReopenedOn,
                round.EffectiveBettingPercentage,
                round.AdditionalData)).ConfigureAwait(false);
        }
    }
}
