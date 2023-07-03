using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;

namespace Tgs.GameRounds.Core.UnitTest;

public class UpdateRoundDetailCommandServiceUnitTest
{
    [Fact]
    public async Task ExternalRoundId_Not_Exist()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();

        var command = new UpdateRoundDetailCommand(
            RoundId: Guid.NewGuid(),
            AdditionalData: "{\"externalRoundId\": \"50-1687319019-331826368-2\"}");

        _ = fakeRepository.ExistAsync(command.RoundId).Returns(ValueTask.FromResult(false));

        var service = new UpdateRoundDetailCommandService(
            fakeRepository);

        _ = await Assert.ThrowsAsync<GameRoundDoesNotExistException>(() => service.ExecuteAsync(command).AsTask()).ConfigureAwait(false);

        _ = fakeRepository
            .Received(0)
            .UpdateRoundDetailAsync(
                id: Arg.Is(command.RoundId),
                additionalData: Arg.Is(command.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Normal_Process()
    {
        var fakeRepository = Substitute.For<IGameRoundRepository>();

        var command = new UpdateRoundDetailCommand(
            RoundId: Guid.NewGuid(),
            AdditionalData: "{\"externalRoundId\": \"50-1687319019-331826368-2\"}");

        _ = fakeRepository.ExistAsync(command.RoundId).Returns(ValueTask.FromResult(true));

        var service = new UpdateRoundDetailCommandService(
            fakeRepository);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeRepository
            .Received(1)
            .UpdateRoundDetailAsync(
                id: Arg.Is(command.RoundId),
                additionalData: Arg.Is(command.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>());
    }
}
