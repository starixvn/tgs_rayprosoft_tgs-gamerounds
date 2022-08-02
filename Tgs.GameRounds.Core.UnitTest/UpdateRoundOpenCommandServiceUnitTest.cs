using System;
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
    public class UpdateRoundOpenCommandServiceUnitTest
    {
        [TestMethod]
        public async Task ExternalRoundId_Not_Exist()
        {
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundConvert = Substitute.For<IGameRoundConvert>();
            var fakeLogger = NullLogger<UpdateRoundOpenCommandService>.Instance;

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

            var command = new UpdateRoundOpenCommand(
                RoundId: roundId,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                Commission: 0,
                Turnover: 10m);

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult(false));

            var service = new UpdateRoundOpenCommandService(
                fakeGameRoundRecord,
                fakeGameRound,
                fakeGameRoundConvert,
                fakeLogger);

            _ = await Assert.ThrowsExceptionAsync<GameRoundDoesNotExistException>(() => service.ExecuteAsync(command).AsTask()).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(0).AddAsync(Arg.Any<GameRoundInfo>());
        }

        [TestMethod]
        public async Task Game_Provider_Currency_IDRK_Is_Converted_Into_The_Platform_Currency_IDR()
        {
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundConvert = Substitute.For<IGameRoundConvert>();
            var fakeLogger = NullLogger<UpdateRoundOpenCommandService>.Instance;

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var currencyCode = "CNY";
            var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

            var command = new UpdateRoundOpenCommand(
                RoundId: roundId,
                BetAmount: 1m,
                ValidBetAmount: 1m,
                Commission: 0,
                Turnover: 1m);

            var expected = Guid.NewGuid();

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult<bool>(true));

            _ = fakeGameRound
                .GetByCurrencyInfoAsync(command.RoundId)
                .Returns(ValueTask.FromResult<(Guid GameProviderId, string CurrencyCode)>(new(gameProviderId, currencyCode)));

            var gameRoundConvertRequest = new GameRoundConvertRequest(
                GameProviderId: gameProviderId,
                CurrencyCode: currencyCode,
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: 0,
                Commission: command.Commission,
                Turnover: command.Turnover,
                PlayerWinLoss: 0);
            var gameRoundConvertResult = new GameRoundConvertResult(
                BetAmount: 1000m,
                ValidBetAmount: 1000m,
                SettleAmount: 0,
                Commission: 0m,
                Turnover: 1000m,
                PlayerWinLoss: 0);
            _ = fakeGameRoundConvert.ConvertPlatformRoundsAsync(
                request: gameRoundConvertRequest).Returns(gameRoundConvertResult);

            var updateRoundOpen = new UpdateRoundOpen(
                RoundId: command.RoundId,
                BetAmount: gameRoundConvertResult.BetAmount,
                ValidBetAmount: gameRoundConvertResult.ValidBetAmount,
                Commission: gameRoundConvertResult.Commission,
                Turnover: gameRoundConvertResult.Turnover);
            _ = fakeGameRound.UpdateRoundOpenAsync(updateRoundOpen);

            var gameRound = new GameRoundInfo(
                RoundId: expected,
                ExternalRoundId: externalRoundId,
                GameProviderId: gameProviderId,
                PlayerCode: playerCode,
                GameId: gameId,
                CurrencyCode: currencyCode,
                Status: GameRoundStatus.Open,
                GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId: walletId,
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: null,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn: DateTime.UtcNow,
                ExternalClosedOn: null,
                BetAmount: gameRoundConvertResult.BetAmount,
                ValidBetAmount: gameRoundConvertResult.ValidBetAmount,
                SettleAmount: gameRoundConvertResult.SettleAmount,
                Commission: gameRoundConvertResult.Commission,
                Turnover: gameRoundConvertResult.Turnover,
                CompanyWinLoss: -gameRoundConvertResult.PlayerWinLoss,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Open,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                EffectiveBettingPercentage: null,
                AdditionalData: string.Empty);
            _ = fakeGameRound.GetAsync(command.RoundId).Returns(ValueTask.FromResult<GameRoundInfo?>(gameRound));

            var service = new UpdateRoundOpenCommandService(
                fakeGameRoundRecord,
                fakeGameRound,
                fakeGameRoundConvert,
                fakeLogger);

            await service.ExecuteAsync(command).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Is(gameRound));
        }

        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();
            var fakeGameRound = Substitute.For<IGameRound>();
            var fakeGameRoundConvert = Substitute.For<IGameRoundConvert>();
            var fakeLogger = NullLogger<UpdateRoundOpenCommandService>.Instance;

            var roundId = Guid.NewGuid();
            var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
            var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
            var gameId = 500001;
            var playerCode = "4ues8xikza7o";
            var currencyCode = "CNY";
            var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

            var command = new UpdateRoundOpenCommand(
                RoundId: roundId,
                BetAmount: 10m,
                ValidBetAmount: 10m,
                Commission: 0,
                Turnover: 10m);

            var expected = Guid.NewGuid();

            _ = fakeGameRound.ExistAsync(command.RoundId).Returns(ValueTask.FromResult<bool>(true));

            _ = fakeGameRound
                .GetByCurrencyInfoAsync(command.RoundId)
                .Returns(ValueTask.FromResult<(Guid GameProviderId, string CurrencyCode)>(new(gameProviderId, currencyCode)));

            var gameRoundConvertRequest = new GameRoundConvertRequest(
                GameProviderId: gameProviderId,
                CurrencyCode: currencyCode,
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: 0,
                Commission: command.Commission,
                Turnover: command.Turnover,
                PlayerWinLoss: 0);
            var gameRoundConvertResult = new GameRoundConvertResult(
                BetAmount: command.BetAmount,
                ValidBetAmount: command.ValidBetAmount,
                SettleAmount: 0,
                Commission: command.Commission,
                Turnover: command.Turnover,
                PlayerWinLoss: 0);
            _ = fakeGameRoundConvert.ConvertPlatformRoundsAsync(
                request: gameRoundConvertRequest).Returns(gameRoundConvertResult);

            var updateRoundOpen = new UpdateRoundOpen(
                RoundId: command.RoundId,
                BetAmount: gameRoundConvertResult.BetAmount,
                ValidBetAmount: gameRoundConvertResult.ValidBetAmount,
                Commission: gameRoundConvertResult.Commission,
                Turnover: gameRoundConvertResult.Turnover);
            _ = fakeGameRound.UpdateRoundOpenAsync(updateRoundOpen);

            var gameRound = new GameRoundInfo(
                RoundId: expected,
                ExternalRoundId: externalRoundId,
                GameProviderId: gameProviderId,
                PlayerCode: playerCode,
                GameId: gameId,
                CurrencyCode: currencyCode,
                Status: GameRoundStatus.Open,
                GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
                WalletId: walletId,
                CreatedOn: DateTimeOffset.UtcNow,
                ClosedOn: null,
                UpdatedOn: DateTimeOffset.UtcNow,
                BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                ExternalCreatedOn: DateTime.UtcNow,
                ExternalClosedOn: null,
                BetAmount: gameRoundConvertResult.BetAmount,
                ValidBetAmount: gameRoundConvertResult.ValidBetAmount,
                SettleAmount: gameRoundConvertResult.SettleAmount,
                Commission: gameRoundConvertResult.Commission,
                Turnover: gameRoundConvertResult.Turnover,
                CompanyWinLoss: -gameRoundConvertResult.PlayerWinLoss,
                TieBetAdjustment: null,
                ClientStatus: GameRoundClientStatus.Open,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: null,
                EffectiveBettingPercentage: null,
                AdditionalData: string.Empty);
            _ = fakeGameRound.GetAsync(command.RoundId).Returns(ValueTask.FromResult<GameRoundInfo?>(gameRound));

            var service = new UpdateRoundOpenCommandService(
                fakeGameRoundRecord,
                fakeGameRound,
                fakeGameRoundConvert,
                fakeLogger);

            await service.ExecuteAsync(command).ConfigureAwait(false);

            _ = fakeGameRoundRecord.Received(1).AddAsync(Arg.Is(gameRound));
        }
    }
}
