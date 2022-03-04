using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateRoundCommandService : ICommandService<UpdateRoundCommand>

    {
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly IGameRoundRepository _gameRoundRepository;

        public UpdateRoundCommandService(IGameRoundRecordRepository gameRoundRecordRepository, IGameRoundRepository gameRoundRepository)
        {
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRoundRepository = gameRoundRepository ?? throw new ArgumentNullException(nameof(gameRoundRepository));
        }

        public async ValueTask ExecuteAsync(UpdateRoundCommand command, CancellationToken cancellationToken = default)
        {
            await _gameRoundRepository.UpdateRoundAsync(command).ConfigureAwait(false);

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
