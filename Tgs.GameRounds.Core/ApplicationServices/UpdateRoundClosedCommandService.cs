using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class UpdateRoundClosedCommandService : ICommandService<UpdateRoundClosedCommand>
{
    private readonly IGameRound _gameRound;
    private readonly IGameRoundRecordMessage _gameRoundRecordMessage;
    private readonly ILogger<UpdateRoundClosedCommandService> _logger;

    public UpdateRoundClosedCommandService(
        ILogger<UpdateRoundClosedCommandService> logger,
        IGameRound gameRound,
        IGameRoundRecordMessage gameRoundRecordMessage)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
        _gameRoundRecordMessage = gameRoundRecordMessage ?? throw new ArgumentNullException(nameof(gameRoundRecordMessage));
    }

    public async ValueTask ExecuteAsync(UpdateRoundClosedCommand command, CancellationToken cancellationToken = default)
    {
        if (!await _gameRound.ExistAsync(
            roundId: command.RoundId,
            cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundDoesNotExistException", DateTime.Now.ToString("o"), command);
            throw new GameRoundDoesNotExistException();
        }

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

        await _gameRound.UpdateRoundClosedAsync(
            roundId: command.RoundId,
            source: updateRoundClosed,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await _gameRoundRecordMessage.SenderGameRoundRecord(
            roundId: command.RoundId,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
