using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class AddCompanyRoundCommandService : ICommandService<AddCompanyRoundCommand>
{
    private readonly IGameRoundRepository _gameRound;
    private readonly IGameRoundLocked _gameRoundLocked;

    public AddCompanyRoundCommandService(
        IGameRoundRepository gameRound,
        IGameRoundLocked gameRoundLocked)
    {
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
        _gameRoundLocked = gameRoundLocked ?? throw new ArgumentNullException(nameof(gameRoundLocked));
    }

    public async ValueTask ExecuteAsync(AddCompanyRoundCommand command, CancellationToken cancellationToken = default)
    {
        var roundId = await _gameRound
            .GetRoundIdAsync(
                gameProviderId: command.GameProviderId,
                externalRoundId: command.ExternalRoundId,
                gameId: command.GameId,
                walletId: command.WalletId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (roundId.HasValue)
        {
            throw new GameRoundCompanyOrderDuplicateException(
                gameProviderId: command.GameProviderId,
                externalRoundId: command.ExternalRoundId,
                gameId: command.GameId,
                walletId: command.WalletId);
        }

        if (!await _gameRoundLocked
            .LockingAsync(
                gameProviderId: command.GameProviderId,
                externalRoundId: command.ExternalRoundId,
                gameId: command.GameId,
                walletId: command.WalletId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false))
        {
            throw new GameRoundLockedException(
                gameProviderId: command.GameProviderId,
                externalRoundId: command.ExternalRoundId,
                gameId: command.GameId,
                walletId: command.WalletId);
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
                throw new GameRoundCreationFailedException(
                    gameProviderId: command.GameProviderId,
                    externalRoundId: command.ExternalRoundId,
                    gameId: command.GameId,
                    walletId: command.WalletId);
            }
        }
        finally
        {
            await _gameRoundLocked
                .UnlockAsync(
                    gameProviderId: command.GameProviderId,
                    externalRoundId: command.ExternalRoundId,
                    gameId: command.GameId,
                    walletId: command.WalletId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
