using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateRoundClosedCommandService : ICommandService<UpdateRoundClosedCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly ILogger<UpdateRoundClosedCommandService> _logger;

        public UpdateRoundClosedCommandService(
            IGameRoundRecordRepository gameRoundRecordRepository,
            IGameRound gameRound,
            ILogger<UpdateRoundClosedCommandService> logger)
        {
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(UpdateRoundClosedCommand command, CancellationToken cancellationToken = default)
        {
            if (!await _gameRound.ExistAsync(command.RoundId).ConfigureAwait(false))
            {
                _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundDoesNotExistException", DateTime.Now.ToString("o"), command);
                throw new GameRoundDoesNotExistException();
            }

            await _gameRound.UpdateRoundClosedAsync(new UpdateRoundClosed(
                RoundId: command.RoundId,
                ExternalClosedOn: command.ExternalClosedOn,
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: command.SettleAmount,
                Commission: command.Commission,
                Turnover: command.Turnover,
                WinLoss: command.WinLoss,
                ClientStatus: command.ClientStatus,
                AdditionalData: command.AdditionalData)).ConfigureAwait(false);

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
