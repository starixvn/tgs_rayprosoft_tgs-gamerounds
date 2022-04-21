using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateRoundClosedCommandService : ICommandService<UpdateRoundClosedCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;

        public UpdateRoundClosedCommandService(
          IGameRoundRecordRepository gameRoundRecordRepository,
          IGameRound gameRound)
        {
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
        }

        public async ValueTask ExecuteAsync(UpdateRoundClosedCommand command, CancellationToken cancellationToken = default)
        {
            if (!await _gameRound.ExistAsync(command.RoundId).ConfigureAwait(false))
            {
                return;
            }

            await _gameRound.UpdateRoundClosedAsync(new(
                command.RoundId,
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

            var round = await _gameRound.GetAsync(command.RoundId).ConfigureAwait(false);

            if (round is not null)
            {
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
}
