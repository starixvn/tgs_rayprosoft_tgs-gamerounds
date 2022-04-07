using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateRoundReopenCommandService : ICommandService<UpdateRoundReopenCommand>
    {
        private readonly IGameRoundService _gameRoundCacheService;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly IGameRoundRepository _gameRoundRepository;

        public UpdateRoundReopenCommandService(
          IGameRoundService gameRoundCacheService,
          IGameRoundRecordRepository gameRoundRecordRepository,
          IGameRoundRepository gameRoundRepository)
        {
            _gameRoundCacheService = gameRoundCacheService ?? throw new ArgumentNullException(nameof(gameRoundCacheService));
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
        }

        public async ValueTask ExecuteAsync(UpdateRoundReopenCommand command, CancellationToken cancellationToken = default)
        {
            if (!await _gameRoundCacheService.ExistAsync(command.RoundId).ConfigureAwait(false))
            {
                return;
            }

            await _gameRoundRepository.UpdateRoundReopenAsync(new(
            command.RoundId,
            command.Status,
            command.ClosedOn,
            command.UpdatedOn,
            command.ExternalClosedOn,
            command.BetAmount,
            command.ValidBetAmount,
            command.SettleAmount,
            command.Commission,
            command.Turnover,
            command.WinLoss,
            command.TieBetAdjustment,
            command.ClientStatus,
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
