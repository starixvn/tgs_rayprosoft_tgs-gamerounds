using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class AddRoundCommandService : ICommandService<AddRoundCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;
        private readonly ILogger<AddRoundCommandService> _logger;

        public AddRoundCommandService(
            IGameRoundRecordRepository gameRoundRecordRepository,
            IGameRound gameRound,
            ILogger<AddRoundCommandService> logger)
        {
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(AddRoundCommand command, CancellationToken cancellationToken = default)
        {
            var roundId = await _gameRound.GetRoundIdAsync(command.GameProviderId, command.ExternalRoundId, command.GameId, command.WalletId).ConfigureAwait(false);
            if (roundId.HasValue)
            {
                return;
            }

            var newRoundId = await _gameRound.AddRoundAsync(
                new AddRound(
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
                    BetAmount: command.BetAmount,
                    ValidBetAmount: command.ValidBetAmount,
                    SettleAmount: command.SettleAmount,
                    Commission: command.Commission,
                    Turnover: command.Turnover,
                    PlayerWinLoss: command.PlayerWinLoss,
                    ClientStatus: command.ClientStatus,
                    AdditionalData: command.AdditionalData)).ConfigureAwait(false);

            if (!newRoundId.HasValue)
            {
                _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundCreationFailedException", DateTime.Now.ToString("o"), command);
                throw new GameRoundCreationFailedException();
            }
            else
            {
                var round = await _gameRound.GetAsync(newRoundId.Value).ConfigureAwait(false);

                if (round is not null)
                {
                    await _gameRoundRecordRepository.AddAsync(round).ConfigureAwait(false);
                }
            }
        }
    }
}
