using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;

namespace Tgs.GameRounds.Core.UnitTest
{
    [TestClass]
    public class UpdateStatusCommandServiceUnitTest
    {
        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRound>();

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

            var command = new UpdateStatusCommand(
                RoundId: roundId,
                Status: GameRoundStatus.Closed,
                ExternalClosedOn: DateTimeOffset.UtcNow);

            var expected = Guid.NewGuid();

            var gameRound = new GameRoundInfo(
                RoundId: expected,
                ExternalRoundId: externalRoundId,
                GameProviderId: gameProviderId,
                PlayerCode: playerCode,
                GameId: gameId,
                CurrencyCode: "CNY",
                Status: GameRoundStatus.Open,
                GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId: walletId,
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: null,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn: DateTime.UtcNow,
                ExternalClosedOn: null,
                BetAmount: 20m,
                ValidBetAmount: 20m,
                SettleAmount: 40m,
                Commission: 0m,
                Turnover: 20m,
                WinLoss: 20m,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Win,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                EffectiveBettingPercentage: null,
                AdditionalData: string.Empty
                );

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult<bool>(true));

            _ = fakeGameRound.UpdateStatusAsync(
                command.RoundId,
                command.Status,
                command.ExternalClosedOn);

            _ = fakeGameRound.GetAsync(command.RoundId).Returns(ValueTask.FromResult<GameRoundInfo?>(gameRound));

            var service = new UpdateStatusCommandService(
                fakeGameRoundRecord,
                fakeGameRound);

            await service.ExecuteAsync(command).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Is(gameRound));
        }
    }
}
