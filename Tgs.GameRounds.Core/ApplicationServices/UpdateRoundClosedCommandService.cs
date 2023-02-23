using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class UpdateRoundClosedCommandService : ICommandService<UpdateRoundClosedCommand>
{
    private readonly IGameRoundRepository _gameRound;

    public UpdateRoundClosedCommandService(
        IGameRoundRepository gameRound)
    {
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
    }

    public ValueTask ExecuteAsync(UpdateRoundClosedCommand command, CancellationToken cancellationToken = default)
    {
        var updateRoundClosed = new UpdateRoundClosed(
            ExternalClosedOn: command.ExternalClosedOn,
            BetAmount: command.BetAmount,
            ValidBetAmount: command.ValidBetAmount,
            SettleAmount: command.SettleAmount,
            Commission: command.Commission,
            Turnover: command.Turnover,
            PlayerWinLoss: command.PlayerWinLoss,
            ClientStatus: command.ClientStatus,
            AdditionalData: command.AdditionalData);

        return _gameRound
            .UpdateRoundClosedAsync(
            id: command.RoundId,
            source: updateRoundClosed,
            cancellationToken: cancellationToken);
    }
}
