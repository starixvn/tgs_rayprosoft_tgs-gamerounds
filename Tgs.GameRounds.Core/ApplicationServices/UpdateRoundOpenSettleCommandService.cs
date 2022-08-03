using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateRoundOpenSettleCommandService : ICommandService<UpdateRoundOpenSettleCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundConvert _gameRoundConvert;
        private readonly IGameRoundRecordMessage _gameRoundRecordMessage;
        private readonly ILogger<UpdateRoundOpenSettleCommandService> _logger;

        public UpdateRoundOpenSettleCommandService(
            ILogger<UpdateRoundOpenSettleCommandService> logger,
            IGameRound gameRound,
            IGameRoundRecordMessage gameRoundRecordMessage,
            IGameRoundConvert gameRoundConvert)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _gameRoundRecordMessage = gameRoundRecordMessage ?? throw new ArgumentNullException(nameof(gameRoundRecordMessage));
            _gameRoundConvert = gameRoundConvert ?? throw new ArgumentNullException(nameof(gameRoundConvert));
        }

        public async ValueTask ExecuteAsync(UpdateRoundOpenSettleCommand command, CancellationToken cancellationToken = default)
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

            var updateRoundOpenSettle = new UpdateRoundOpenSettle(
                RoundId: command.RoundId,
                BetAmount: gameRoundConvertResult.BetAmount,
                ValidBetAmount: gameRoundConvertResult.ValidBetAmount,
                SettleAmount: gameRoundConvertResult.SettleAmount,
                Commission: gameRoundConvertResult.Commission,
                Turnover: gameRoundConvertResult.Turnover,
                PlayerWinLoss: gameRoundConvertResult.PlayerWinLoss);

            await _gameRound.UpdateRoundOpenSettleAsync(
                source: updateRoundOpenSettle,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await _gameRoundRecordMessage.SenderGameRoundRecord(
                roundId: command.RoundId,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
