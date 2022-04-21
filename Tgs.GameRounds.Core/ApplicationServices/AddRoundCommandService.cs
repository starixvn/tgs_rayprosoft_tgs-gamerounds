using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    internal class AddRoundCommandService : ICommandService<AddRoundCommand>
    {
        private readonly IGameRound _gameRound;
        private readonly IGameRoundRecordRepository _gameRoundRecordRepository;

        public AddRoundCommandService(
            IGameRoundRecordRepository gameRoundRecordRepository,
            IGameRound gameRound)
        {
            _gameRoundRecordRepository = gameRoundRecordRepository ?? throw new ArgumentNullException(nameof(gameRoundRecordRepository));
            _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
        }

        public async ValueTask ExecuteAsync(AddRoundCommand command, CancellationToken cancellationToken = default)
        {
            var roundId = await _gameRound.GetRoundIdAsync(command.GameProviderId, command.ExternalRoundId, command.GameId, command.WalletId).ConfigureAwait(false);
            if (roundId.HasValue)
            {
                return;
            }

            var newRoundId = await _gameRound.AddRoundAsync(
                new AddRound(
                    ExternalRoundId: command.ExternalRoundId,
                    GameProviderId: command.GameProviderId,
                    PlayerCode: command.PlayerCode,
                    GameId: command.GameId,
                    CurrencyCode: command.CurrencyCode,
                    Status: command.Status,
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
                    WinLoss: command.WinLoss,
                    ClientStatus: command.ClientStatus,
                    AdditionalData: command.AdditionalData)).ConfigureAwait(false);

            if (!newRoundId.HasValue)
            {
                throw new Exception("This GameRound failed to create an order.");
            }
            else
            {
                var round = await _gameRound.GetAsync(newRoundId.Value).ConfigureAwait(false);

                if (round is not null)
                {
                    await _gameRoundRecordRepository.AddAsync(new(
                        round.RoundId,
                        round.ExternalRoundId,
                        round.GameProviderId,
                        round.PlayerCode,
                        round.GameId,
                        round.CurrencyCode,
                        round.Status,
                        round.GameProviderConfigurationId,
                        round.WalletId,
                        round.CreatedOn,
                        round.ClosedOn,
                        round.UpdatedOn,
                        round.BrandId,
                        round.ExternalCreatedOn,
                        round.ExternalClosedOn,
                        round.BetAmount,
                        round.ValidBetAmount,
                        round.SettleAmount,
                        round.Commission,
                        round.Turnover,
                        round.WinLoss,
                        round.TieBetAdjustment,
                        round.ClientStatus,
                        round.ReopenTurnoverAdjustment,
                        round.ReopenedOn,
                        round.EffectiveBettingPercentage,
                        round.AdditionalData)).ConfigureAwait(false);
                }
            }
        }
    }
}
