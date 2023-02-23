using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class ManualCloseRoundCommandService : ICommandService<ManualCloseRoundCommand>
{
    private readonly IGameRoundRepository _gameRound;
    private readonly ILogger<ManualCloseRoundCommandService> _logger;

    public ManualCloseRoundCommandService(
        ILogger<ManualCloseRoundCommandService> logger,
        IGameRoundRepository gameRound)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
    }

    public async ValueTask ExecuteAsync(ManualCloseRoundCommand command, CancellationToken cancellationToken = default)
    {
        if (!await _gameRound
            .ExistAsync(
                id: command.RoundId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false))
        {
            _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundDoesNotExistException", DateTime.Now.ToString("o"), command);
            throw new GameRoundDoesNotExistException();
        }

        var status = await _gameRound
                    .GetStatusAsync(
                        id: command.RoundId,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

        if (status != GameRoundStatus.Open)
        {
            throw new GameRoundStatusException(status);
        }

        var manualClose = new ManualCloseRound(
            ExternalClosedOn: command.ExternalClosedOn,
            BetAmount: command.BetAmount,
            ValidBetAmount: command.ValidBetAmount,
            SettleAmount: command.SettleAmount,
            Commission: command.Commission,
            Turnover: command.Turnover,
            CompanyWinLoss: command.CompanyWinLoss,
            ClientStatus: command.ClientStatus,
            OperatorId: command.OperatorId,
            Remark: command.Remark);

        await _gameRound.ManualCloseRoundAsync(
            id: command.RoundId,
            source: manualClose,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
