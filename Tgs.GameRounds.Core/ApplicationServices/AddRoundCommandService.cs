using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class AddRoundCommandService : ICommandService<AddRoundCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundConvert _gameRoundConvert;
        private readonly IGameRoundLocked _gameRoundLocked;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly ILogger<AddRoundCommandService> _logger;

        public AddRoundCommandService(
            ILogger<AddRoundCommandService> logger,
            IGameRound gameRound,
            IGameRoundConvert gameRoundConvert,
            IGameRoundLocked gameRoundLocked,
            IGameRoundRecordRepository gameRoundRecordRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _gameRoundConvert = gameRoundConvert ?? throw new ArgumentNullException(nameof(gameRoundConvert));
            _gameRoundLocked = gameRoundLocked ?? throw new ArgumentNullException(nameof(gameRoundLocked));
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
        }

        public async ValueTask ExecuteAsync(AddRoundCommand command, CancellationToken cancellationToken = default)
        {
            var roundId = await _gameRound.GetRoundIdAsync(
                    gameProviderId: command.GameProviderId,
                    externalRoundId: command.ExternalRoundId,
                    gameId: command.GameId,
                    walletId: command.WalletId,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            if (roundId.HasValue)
            {
                return;
            }

            if (!await _gameRoundLocked.LockingAsync(
                    gameProviderId: command.GameProviderId,
                    externalRoundId: command.ExternalRoundId,
                    gameId: command.GameId,
                    walletId: command.WalletId,
                    cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                return;
            }

            try
            {
                var gameRoundConvertRequest = new GameRoundConvertRequest(
                     GameProviderId: command.GameProviderId,
                     CurrencyCode: command.CurrencyCode,
                     BetAmount: command.BetAmount,
                     ValidBetAmount: command.ValidBetAmount,
                     SettleAmount: command.SettleAmount.GetValueOrDefault(0),
                     Commission: command.Commission,
                     Turnover: command.Turnover,
                     PlayerWinLoss: command.PlayerWinLoss.GetValueOrDefault(0));

                var gameRoundConvertResult = await _gameRoundConvert.ConvertPlatformRoundsAsync(
                    request: gameRoundConvertRequest,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                var addRound = new AddRound(
                    ExternalRoundId: command.ExternalRoundId,
                    GameProviderId: command.GameProviderId,
                    PlayerCode: command.PlayerCode,
                    GameId: command.GameId,
                    CurrencyCode: command.CurrencyCode,
                    Status: command.Status,
                    GameProviderConfigurationId: command.GameProviderConfigurationId,
                    WalletId: command.WalletId,
                    BrandId: command.BrandId,
                    ExternalCreatedOn: command.ExternalCreatedOn,
                    ExternalClosedOn: command.ExternalClosedOn,
                    BetAmount: gameRoundConvertResult.BetAmount,
                    ValidBetAmount: gameRoundConvertResult.ValidBetAmount,
                    SettleAmount: gameRoundConvertResult.SettleAmount,
                    Commission: gameRoundConvertResult.Commission,
                    Turnover: gameRoundConvertResult.Turnover,
                    PlayerWinLoss: gameRoundConvertResult.PlayerWinLoss,
                    ClientStatus: command.ClientStatus,
                    AdditionalData: command.AdditionalData);

                var newRoundId = await _gameRound.AddRoundAsync(
                    source: addRound,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!newRoundId.HasValue)
                {
                    _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundCreationFailedException", DateTime.Now.ToString("o"), command);
                    throw new GameRoundCreationFailedException();
                }
                else
                {
                    var round = await _gameRound.GetAsync(
                        roundId: newRoundId.Value,
                        cancellationToken: cancellationToken).ConfigureAwait(false);

                    if (round is not null)
                    {
                        await _gameRoundRecordRepository.AddAsync(
                            gameRound: round,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                await _gameRoundLocked.UnlockAsync(
                        gameProviderId: command.GameProviderId,
                        externalRoundId: command.ExternalRoundId,
                        gameId: command.GameId,
                        walletId: command.WalletId,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
