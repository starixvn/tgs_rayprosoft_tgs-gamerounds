using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class UpdateRoundReopenCommandService : ICommandService<UpdateRoundReopenCommand>
{
    private readonly IGameRoundRepository _gameRound;
    private readonly IGameRoundRecordMessage _gameRoundRecordMessage;
    private readonly ILogger<UpdateRoundReopenCommandService> _logger;

    public UpdateRoundReopenCommandService(
        ILogger<UpdateRoundReopenCommandService> logger,
        IGameRoundRepository gameRound,
        IGameRoundRecordMessage gameRoundRecordMessage)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
        _gameRoundRecordMessage = gameRoundRecordMessage ?? throw new ArgumentNullException(nameof(gameRoundRecordMessage));
    }

    public async ValueTask ExecuteAsync(UpdateRoundReopenCommand command, CancellationToken cancellationToken = default)
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

        await _gameRound.UpdateRoundReopenAsync(
            id: command.RoundId,
            source: updateRoundReopen,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await _gameRoundRecordMessage.SenderGameRoundRecord(
            roundId: command.RoundId,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
