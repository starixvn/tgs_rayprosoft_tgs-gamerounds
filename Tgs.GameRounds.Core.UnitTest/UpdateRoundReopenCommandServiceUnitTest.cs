using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.UnitTest
{
    [TestClass]
    public class UpdateRoundReopenCommandServiceUnitTest
    {
        [TestMethod]
        public async Task ExternalRoundId_Not_Exist()
        {
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeLogger = NullLogger<UpdateRoundReopenCommandService>.Instance;

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

            var command = new UpdateRoundReopenCommand(
                RoundId: roundId,
                Status: GameRoundStatus.Closed,
                ExternalClosedOn: DateTimeOffset.UtcNow,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                SettleAmount: 20m,
                Commission: 0,
                Turnover: 10m,
                PlayerWinLoss: 10m,
                ClientStatus: GameRoundClientStatus.Win,
                AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult(false));

            var service = new UpdateRoundReopenCommandService(
                fakeGameRoundRecord,
                fakeGameRound,
                fakeLogger);

            _ = await Assert.ThrowsExceptionAsync<GameRoundDoesNotExistException>(() => service.ExecuteAsync(command).AsTask()).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(0).AddAsync(Arg.Any<GameRoundInfo>());
        }

        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeLogger = NullLogger<UpdateRoundReopenCommandService>.Instance;

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

            var command = new UpdateRoundReopenCommand(
                RoundId: roundId,
                Status: GameRoundStatus.Closed,
                ExternalClosedOn: DateTimeOffset.UtcNow,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                SettleAmount: 20m,
                Commission: 0,
                Turnover: 10m,
                PlayerWinLoss: 10m,
                ClientStatus: GameRoundClientStatus.Win,
                AdditionalData: JsonSerializer.Serialize(new { RoundId = roundId }));

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
                BetAmount: null,
                ValidBetAmount: null,
                SettleAmount: null,
                Commission: null,
                Turnover: null,
                CompanyWinLoss: null,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Open,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                EffectiveBettingPercentage: null,
                AdditionalData: string.Empty
                );

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult<bool>(true));

            _ = fakeGameRound.UpdateRoundReopenAsync(new UpdateRoundReopen(
                RoundId: command.RoundId,
                Status: command.Status,
                ExternalClosedOn: command.ExternalClosedOn,
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: command.SettleAmount,
                Commission: command.Commission,
                Turnover: command.Turnover,
                PlayerWinLoss: command.PlayerWinLoss,
                ClientStatus: command.ClientStatus,
                AdditionalData: command.AdditionalData));

            _ = fakeGameRound.GetAsync(command.RoundId).Returns(ValueTask.FromResult<GameRoundInfo?>(gameRound));

            var service = new UpdateRoundReopenCommandService(
                fakeGameRoundRecord,
                fakeGameRound,
                fakeLogger);

            await service.ExecuteAsync(command).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Is(gameRound));
        }
    }
}
