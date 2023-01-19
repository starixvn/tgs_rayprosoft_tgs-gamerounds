using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.ApplicationServices;

namespace Tgs.GameRounds.Core.UnitTest;

[TestClass]
public class GetGameRoundByExternalGameGetServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeGameRound = Substitute.For<IGameRound>();

        var roundId = Guid.NewGuid();

        var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
        var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
        var externalGameId = "Bacc";
        var gameId = 500001;
        var walletId = Guid.NewGuid();
        var gameProviderCode = "SG";
        var gameRound = new GameRoundInfo(
            RoundId: roundId,
            ExternalRoundId: externalRoundId,
            GameProviderId: gameProviderId,
            PlayerCode: "test",
            GameId: gameId,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Open,
            GameProviderConfigurationId: Guid.NewGuid(),
            WalletId: walletId,
            CreatedOn: DateTimeOffset.UtcNow,
            ClosedOn: null,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: Guid.NewGuid(),
            ExternalCreatedOn: null,
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
            AdditionalData: string.Empty);

        var query = new GetGameRoundByExternalGameQuery(gameProviderId, externalRoundId, externalGameId, walletId);

        _ = fakeGame.GetGameIdAsync(query.GameProviderId, query.ExternalGameId).Returns(ValueTask.FromResult(gameId));
        _ = fakeGameProvider.GetGameProviderCodeAsync(query.GameProviderId).Returns(ValueTask.FromResult(gameProviderCode));

        _ = fakeGameRound.GetAsync(query.GameProviderId, query.ExternalRoundId, gameId, query.WalletId).Returns(ValueTask.FromResult<GameRoundInfo?>(gameRound));

        var service = new GetGameRoundByExternalGameGetService(
            fakeGame,
            fakeGameProvider,
            fakeGameRound);

        var actual = await service.GetAsync(query).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        _ = Equals(gameRound, actual);
    }
}
