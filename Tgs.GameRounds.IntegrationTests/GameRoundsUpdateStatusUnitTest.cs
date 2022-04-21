using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.IntegrationTests
{
    public partial class GameRoundsClientUnitTest
    {
        [TestMethod]
        public async Task GameRounds_UpdateStatus_Normal_Process()
        {
            var roundId = Guid.NewGuid();

            var status = GameRoundStatus.Closed;

            var externalClosedOn = DateTimeOffset.UtcNow;

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            var web = CreateTestSettingSite(services =>
            {
                var fakeService = Substitute.For<ICommandService<UpdateStatusCommand>>();
                _ = fakeService.ExecuteAsync(new UpdateStatusCommand(roundId, status, externalClosedOn)).Returns(ValueTask.CompletedTask);
                _ = services.AddSingleton(fakeService);

                _ = services.AddSingleton(fakeGameRoundRecord);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            await sut.UpdateStatusAsync(roundId, status, externalClosedOn).ConfigureAwait(false);
        }
    }
}
