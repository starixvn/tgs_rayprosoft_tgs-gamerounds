using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class UpdateRoundOpenCommandService : ICommandService<UpdateRoundOpenCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundConvert _gameRoundConvert;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly ILogger<UpdateRoundOpenCommandService> _logger;

        public UpdateRoundOpenCommandService(
          IGameRoundRecordRepository gameRoundRecordRepository,
          IGameRound gameRound,
          IGameRoundConvert gameRoundConvert,
          ILogger<UpdateRoundOpenCommandService> logger)
        {
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _gameRoundConvert = gameRoundConvert ?? throw new ArgumentNullException(nameof(gameRoundConvert));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(UpdateRoundOpenCommand command, CancellationToken cancellationToken = default)
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
                 SettleAmount: 0,
                 Commission: command.Commission,
                 Turnover: command.Turnover,
                 PlayerWinLoss: 0);

            var gameRoundConvertResult = await _gameRoundConvert.ConvertPlatformRoundsAsync(
                request: gameRoundConvertRequest,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var updateRoundOpen = new UpdateRoundOpen(
                RoundId: command.RoundId,
                BetAmount: gameRoundConvertResult.BetAmount,
                ValidBetAmount: gameRoundConvertResult.ValidBetAmount,
                Commission: gameRoundConvertResult.Commission,
                Turnover: gameRoundConvertResult.Turnover);

            await _gameRound.UpdateRoundOpenAsync(
                source: updateRoundOpen,
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
