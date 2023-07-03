using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.IntegrationTests;

public partial class GameRoundsClientUnitTest
{
    [Fact]
    public async Task GameRounds_UpdateRoundDetail_ExternalRoundId_Not_Exist()
    {
        var roundId = Guid.NewGuid();

        var additionalData = "{\"externalRoundId\": \"50-1687319019-331826368-2\"}";

        var web = CreateTestSettingSite(services =>
        {
            var fakeService = Substitute.For<ICommandService<UpdateRoundDetailCommand>>();

            _ = fakeService
            .ExecuteAsync(
                command: Arg.Any<UpdateRoundDetailCommand>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Throws(new GameRoundDoesNotExistException(roundId));

            _ = services.AddSingleton(fakeService);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        _ = await Assert
            .ThrowsAsync<HttpRequestException>(
            () =>
            sut
            .UpdateRoundDetailAsync(
                roundId: roundId,
                additionalData: additionalData)
            .AsTask())
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task GameRounds_UpdateRoundDetail_Normal_Process()
    {
        var roundId = Guid.NewGuid();

        var additionalData = "{\"externalRoundId\": \"50-1687319019-331826368-2\"}";

        var web = CreateTestSettingSite(services =>
        {
            var fakeService = Substitute.For<ICommandService<UpdateRoundDetailCommand>>();

            _ = fakeService.ExecuteAsync(Arg.Any<UpdateRoundDetailCommand>()).Returns(ValueTask.CompletedTask);

            _ = services.AddSingleton(fakeService);
        });

        var httpClient = web.CreateClient();

        var sut = new GameRoundsClient(httpClient);

        await sut
            .UpdateRoundDetailAsync(
            roundId: roundId,
            additionalData: additionalData)
            .ConfigureAwait(false);
    }
}
