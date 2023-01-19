using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

public class AddGameRoundRecordCommandService : ICommandService<AddGameRoundRecordCommand>
{
    private readonly IGame _game;
    private readonly IGameProvider _gameProvider;
    private readonly IGameRound _gameRound;
    private readonly IGameRoundRecord _gameRoundRecord;
    private readonly ILogger<AddGameRoundRecordCommandService> _logger;

    public AddGameRoundRecordCommandService(
        ILogger<AddGameRoundRecordCommandService> logger,
        IGame game,
        IGameProvider gameProvider,
        IGameRound gameRound,
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

        var gameProviderCode = await _gameProvider.GetGameProviderCodeAsync(
             gameProviderId: round.GameProviderId,
             cancellationToken: cancellationToken).ConfigureAwait(false);

        var externalGameId = await _game.GetExternalGameIdAsync(
            gameId: round.GameId,
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
            Bet: round.BetAmount.GetValueOrDefault(0),
            ValidBet: round.ValidBetAmount.GetValueOrDefault(0),
            Settle: round.SettleAmount.GetValueOrDefault(0),
            Commission: round.Commission.GetValueOrDefault(0),
            Turnover: round.Turnover.GetValueOrDefault(0),
            CompanyWinLoss: round.CompanyWinLoss.GetValueOrDefault(0),
            TieBetAdjustment: round.TieBetAdjustment.GetValueOrDefault(0),
            ReopenTurnoverAdjustment: round.ReopenTurnoverAdjustment.GetValueOrDefault(0),
            ReopenedOn: round.ReopenedOn,
            SendOn: command.SendOn,
            ReceiveOn: command.ReceiveOn);

        await _gameRoundRecord.AddAsync(
            add: gameRoundRecordAdd,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
