using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.IntegrationTests;

public partial class GameRoundsClientUnitTest
{
    [TestMethod]
    public async Task GameRounds_AdjustClosedRound_ExternalRoundId_Not_Exist()
    {
        var roundId = Guid.NewGuid();

        var model = new AdjustClosedRoundViewModel()
        {
            BetAmount = 10m,
            ValidBetAmount = 10m,
            SettleAmount = 0,
            Commission = 0,
            Turnover = 10m,
            CompanyWinLoss = 10m,
            ClientStatus = GameRoundClientStatus.Loss,
            ExternalClosedOn = DateTimeOffset.UtcNow,
            OperatorId = Guid.NewGuid(),
            Remark = "test"
        };

        var web = CreateTestSettingSite(services =>
        {
            var fakeService = Substitute.For<ICommandService<AdjustClosedRoundCommand>>();

            _ = fakeService.ExecuteAsync(Arg.Any<AdjustClosedRoundCommand>()).Throws<GameRoundDoesNotExistException>();

            _ = services.AddSingleton(fakeService);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.AdjustClosedRoundAsync(roundId, model).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(false, actual.Success);
        Assert.AreEqual(HttpStatusCode.InternalServerError, actual.HttpStatusCode);
    }

    [TestMethod]
    public async Task GameRounds_AdjustClosedRound_Normal_Process()
    {
        var roundId = Guid.NewGuid();

        var model = new AdjustClosedRoundViewModel()
        {
            BetAmount = 10m,
            ValidBetAmount = 10m,
            SettleAmount = 0,
            Commission = 0,
            Turnover = 10m,
            CompanyWinLoss = 10m,
            ClientStatus = GameRoundClientStatus.Loss,
            ExternalClosedOn = DateTimeOffset.UtcNow,
            OperatorId = Guid.NewGuid(),
            Remark = "test"
        };

        var web = CreateTestSettingSite(services =>
        {
            var fakeService = Substitute.For<ICommandService<AdjustClosedRoundCommand>>();

            _ = fakeService.ExecuteAsync(Arg.Any<AdjustClosedRoundCommand>()).Returns(ValueTask.CompletedTask);

            _ = services.AddSingleton(fakeService);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        var actual = await sut.AdjustClosedRoundAsync(roundId, model).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(true, actual.Success);
        Assert.AreEqual(HttpStatusCode.OK, actual.HttpStatusCode);
    }
}
