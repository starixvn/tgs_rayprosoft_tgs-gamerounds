using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateRoundClosedCommandService : ICommandService<UpdateRoundClosedCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundConvert _gameRoundConvert;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly ILogger<UpdateRoundClosedCommandService> _logger;

        public UpdateRoundClosedCommandService(
            IGameRoundRecordRepository gameRoundRecordRepository,
            IGameRound gameRound,
            IGameRoundConvert gameRoundConvert,
            ILogger<UpdateRoundClosedCommandService> logger)
        {
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _gameRoundConvert = gameRoundConvert ?? throw new ArgumentNullException(nameof(gameRoundConvert));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(UpdateRoundClosedCommand command, CancellationToken cancellationToken = default)
        {
            if (!await _gameRound.ExistAsync(
                roundId: command.RoundId,
                cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundDoesNotExistException", DateTime.Now.ToString("o"), command);
                throw new GameRoundDoesNotExistException();
            }

            var (GameProviderId, CurrencyCode) = await _gameRound
                .GetByCurrencyInfoAsync(
                    roundId: command.RoundId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var gameRoundConvertRequest = new GameRoundConvertRequest(
                 GameProviderId: GameProviderId,
                 CurrencyCode: CurrencyCode,
                 BetAmount: command.BetAmount,
                 ValidBetAmount: command.ValidBetAmount,
                 SettleAmount: command.SettleAmount,
                 Commission: command.Commission,
                 Turnover: command.Turnover,
                 PlayerWinLoss: command.PlayerWinLoss);

            var gameRoundConvertResult = await _gameRoundConvert.ConvertPlatformRoundsAsync(
                request: gameRoundConvertRequest,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var updateRoundClosed = new UpdateRoundClosed(
                RoundId: command.RoundId,
                ExternalClosedOn: command.ExternalClosedOn,
                BetAmount: gameRoundConvertResult.BetAmount,
                ValidBetAmount: gameRoundConvertResult.ValidBetAmount,
                SettleAmount: gameRoundConvertResult.SettleAmount,
                Commission: gameRoundConvertResult.Commission,
                Turnover: gameRoundConvertResult.Turnover,
                PlayerWinLoss: gameRoundConvertResult.PlayerWinLoss,
                ClientStatus: command.ClientStatus,
                AdditionalData: command.AdditionalData);

            await _gameRound.UpdateRoundClosedAsync(
                source: updateRoundClosed,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var round = await _gameRound.GetAsync(
                roundId: command.RoundId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (round is not null)
            {
                await _gameRoundRecordRepository.AddAsync(
                    gameRound: round,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
