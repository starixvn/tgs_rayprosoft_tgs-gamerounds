using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class UpdateRoundReopenCommandService : ICommandService<UpdateRoundReopenCommand>
{
    private readonly IGameRoundRepository _gameRound;

    public UpdateRoundReopenCommandService(
        IGameRoundRepository gameRound)
    {
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
    }

    public ValueTask ExecuteAsync(UpdateRoundReopenCommand command, CancellationToken cancellationToken = default)
    {
        var updateRoundReopen = new UpdateRoundReopen(
            Status: command.Status,
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
            .UpdateRoundReopenAsync(
            id: command.RoundId,
            source: updateRoundReopen,
            cancellationToken: cancellationToken);
    }
}
