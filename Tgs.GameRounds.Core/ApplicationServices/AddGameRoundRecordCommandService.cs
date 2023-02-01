using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

public class AddGameRoundRecordCommandService : ICommandService<AddGameRoundRecordCommand>
{
    private readonly IGameRepository _game;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameRoundRepository _gameRound;
    private readonly IGameRoundRecord _gameRoundRecord;
    private readonly ILogger<AddGameRoundRecordCommandService> _logger;

    public AddGameRoundRecordCommandService(
        ILogger<AddGameRoundRecordCommandService> logger,
        IGameRepository game,
        IGameProviderRepository gameProvider,
        IGameRoundRepository gameRound,
        IGameRoundRecord gameRoundRecord)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
        _gameRoundRecord = gameRoundRecord ?? throw new ArgumentNullException(nameof(gameRoundRecord));
    }

    public async ValueTask ExecuteAsync(AddGameRoundRecordCommand command, CancellationToken cancellationToken = default)
    {
        var round = await _gameRound.GetAsync(
           roundId: command.RoundId,
           cancellationToken: cancellationToken).ConfigureAwait(false);

        if (round is null)
        {
            _logger.LogError("The database could not find the game round data.(roundId)", command.RoundId);
            return;
        }

        var gameProviderCode = await _gameProvider.GetCode(
             id: round.GameProviderId,
             cancellationToken: cancellationToken).ConfigureAwait(false);

        var externalGameId = await _game.GetExternalIdAsync(
            id: round.GameId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var gameRoundRecordAdd = new AddGameRoundRecord(
            RoundId: round.RoundId,
            ExternalRoundId: round.ExternalRoundId,
            ExternalGameId: externalGameId,
            GameProviderId: round.GameProviderId,
            GameProviderCode: gameProviderCode,
            GameProviderConfigurationId: round.GameProviderConfigurationId,
            GameId: round.GameId,
            BrandId: round.BrandId,
            PlayerCode: round.PlayerCode,
            WalletId: round.WalletId,
            CurrencyCode: round.CurrencyCode,
            Status: round.Status,
            ClientStatus: round.ClientStatus,
            CreatedOn: round.CreatedOn,
            ClosedOn: round.ClosedOn,
            UpdatedOn: round.UpdatedOn,
            ExternalCreatedOn: round.ExternalCreatedOn,
            ExternalClosedOn: round.ExternalClosedOn,
            Bet: round.BetAmount,
            ValidBet: round.ValidBetAmount,
            Settle: round.SettleAmount,
            Commission: round.Commission,
            Turnover: round.Turnover,
            CompanyWinLoss: round.CompanyWinLoss,
            TieBetAdjustment: round.TieBetAdjustment,
            ReopenTurnoverAdjustment: round.ReopenTurnoverAdjustment.GetValueOrDefault(0),
            ReopenedOn: round.ReopenedOn,
            SendOn: command.SendOn,
            ReceiveOn: command.ReceiveOn);

        await _gameRoundRecord.AddAsync(
            add: gameRoundRecordAdd,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
