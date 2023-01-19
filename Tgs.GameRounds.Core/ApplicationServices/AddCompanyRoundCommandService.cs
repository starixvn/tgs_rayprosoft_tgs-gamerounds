using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class AddCompanyRoundCommandService : ICommandService<AddCompanyRoundCommand>
{
    private readonly IGameRound _gameRound;
    private readonly IGameRoundLocked _gameRoundLocked;
    private readonly IGameRoundRecordMessage _gameRoundRecordMessage;
    private readonly ILogger<AddCompanyRoundCommandService> _logger;

    public AddCompanyRoundCommandService(
        ILogger<AddCompanyRoundCommandService> logger,
        IGameRound gameRound,
        IGameRoundLocked gameRoundLocked,
        IGameRoundRecordMessage gameRoundRecordMessage)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
        _gameRoundLocked = gameRoundLocked ?? throw new ArgumentNullException(nameof(gameRoundLocked));
        _gameRoundRecordMessage = gameRoundRecordMessage ?? throw new ArgumentNullException(nameof(gameRoundRecordMessage));
    }

    public async ValueTask ExecuteAsync(AddCompanyRoundCommand command, CancellationToken cancellationToken = default)
    {
        var roundId = await _gameRound.GetRoundIdAsync(
                gameProviderId: command.GameProviderId,
                externalRoundId: command.ExternalRoundId,
                gameId: command.GameId,
                walletId: command.WalletId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

        if (roundId.HasValue)
        {
            throw new GameRoundCompanyOrderDuplicateException();
        }

        if (!await _gameRoundLocked.LockingAsync(
                gameProviderId: command.GameProviderId,
                externalRoundId: command.ExternalRoundId,
                gameId: command.GameId,
                walletId: command.WalletId,
                cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            throw new GameRoundCreationFailedException();
        }

        try
        {
            var add = new AddCompanyRound(
                ExternalRoundId: command.ExternalRoundId,
                GameProviderId: command.GameProviderId,
                PlayerCode: command.PlayerCode,
                GameId: command.GameId,
                CurrencyCode: command.CurrencyCode,
                GameProviderConfigurationId: command.GameProviderConfigurationId,
                WalletId: command.WalletId,
                BrandId: command.BrandId,
                ExternalCreatedOn: command.ExternalCreatedOn,
                ExternalClosedOn: command.ExternalClosedOn,
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: command.SettleAmount,
                Commission: command.Commission,
                Turnover: command.Turnover,
                CompanyWinLoss: command.CompanyWinLoss,
                OperatorId: command.OperatorId,
                Remark: command.Remark,
                ClientStatus: command.ClientStatus);

            var newRoundId = await _gameRound.AddCompanyRoundAsync(
                source: add,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!newRoundId.HasValue)
            {
                _logger.LogError("LogOn:{DateTime} Data: {data} Exception:GameRoundCreationFailedException", DateTime.Now.ToString("o"), command);
                throw new GameRoundCreationFailedException();
            }
            else
            {
                await _gameRoundRecordMessage.SenderGameRoundRecord(
                    roundId: newRoundId.Value,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            await _gameRoundLocked.UnlockAsync(
                    gameProviderId: command.GameProviderId,
                    externalRoundId: command.ExternalRoundId,
                    gameId: command.GameId,
                    walletId: command.WalletId,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
