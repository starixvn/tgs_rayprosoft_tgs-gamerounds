using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class UpdateRoundOpenCommandService : ICommandService<UpdateRoundOpenCommand>
{
    private readonly IGameRoundRepository _gameRound;

    public UpdateRoundOpenCommandService(
        IGameRoundRepository gameRound)
    {
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
    }

    public ValueTask ExecuteAsync(UpdateRoundOpenCommand command, CancellationToken cancellationToken = default)
    {
        var updateRoundOpen = new UpdateRoundOpen(
            BetAmount: command.BetAmount,
            ValidBetAmount: command.ValidBetAmount,
            Commission: command.Commission,
            Turnover: command.Turnover);

        return _gameRound
            .UpdateRoundOpenAsync(
            id: command.RoundId,
            source: updateRoundOpen,
            cancellationToken: cancellationToken);
    }
}
