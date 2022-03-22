using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class AddRoundCommandService : ICommandService<AddRoundCommand>
    {
        private readonly IGameRoundService _gameRoundCacheService;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly IGameRoundRepository _gameRoundRepository;

        public AddRoundCommandService(
          IGameRoundService gameRoundCacheService,
          IGameRoundRecordRepository gameRoundRecordRepository,
          IGameRoundRepository gameRoundRepository)
        {
            _gameRoundCacheService = gameRoundCacheService ?? throw new ArgumentNullException(nameof(gameRoundCacheService));
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
        }

        public async ValueTask ExecuteAsync(AddRoundCommand command, CancellationToken cancellationToken = default)
        {
            var roundId = await _gameRoundCacheService.GetRoundIdAsync(command.GameProviderId, command.ExternalRoundId, command.GameId, command.WalletId).ConfigureAwait(false);
            if (roundId.HasValue)
            {
                return;
            }

            var newRoundId = await _gameRoundRepository.AddRoundAsync(
                new AddRound(
                    command.ExternalRoundId,
                    command.GameProviderId,
                    command.PlayerCode,
                    command.GameId,
                    command.CurrencyCode,
                    command.Status,
                    command.GameProviderConfigurationId,
                    command.WalletId,
                    command.CreatedOn,
                    command.ClosedOn,
                    command.UpdatedOn,
                    command.BrandId,
                    command.ExternalCreatedOn,
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

            var roundRecord = await _gameRoundRepository.GetAsync(newRoundId).ConfigureAwait(false);

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
        }
    }
}
