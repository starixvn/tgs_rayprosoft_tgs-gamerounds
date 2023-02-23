using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class AdjustClosedRoundCommandService : ICommandService<AdjustClosedRoundCommand>
{
    private readonly IGameRoundRepository _gameRound;
    private readonly ILogger<AdjustClosedRoundCommandService> _logger;

    public AdjustClosedRoundCommandService(
        ILogger<AdjustClosedRoundCommandService> logger,
        IGameRoundRepository gameRound)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
    }

    public async ValueTask ExecuteAsync(AdjustClosedRoundCommand command, CancellationToken cancellationToken = default)
    {
        if (!await _gameRound.ExistAsync(
            id: command.RoundId,
            cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundDoesNotExistException", DateTime.Now.ToString("o"), command);
            throw new GameRoundDoesNotExistException();
        }

        var status = await _gameRound
                    .GetStatusAsync(
                        id: command.RoundId,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

        if (status != GameRoundStatus.Closed)
        {
            throw new GameRoundStatusException(status);
        }

        var adjustClosed = new AdjustClosedRound(
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

        await _gameRound.AdjustClosedRoundAsync(
            id: command.RoundId,
            source: adjustClosed,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
