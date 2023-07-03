using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class ManualCloseRoundCommandService : ICommandService<ManualCloseRoundCommand>
{
    private readonly IGameRoundRepository _gameRound;

    public ManualCloseRoundCommandService(
        IGameRoundRepository gameRound)
    {
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
            throw new GameRoundDoesNotExistException(command.RoundId);
        }

        var status = await _gameRound
            .GetStatusAsync(
            id: command.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (status != GameRoundStatus.Open)
        {
            throw new GameRoundStatusException(command.RoundId, status);
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
