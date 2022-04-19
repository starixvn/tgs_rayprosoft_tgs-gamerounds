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
        public async Task GameRounds_UpdateClientStatus_Normal_Process()
        {
            var roundId = Guid.NewGuid();

            var status = GameRoundClientStatus.Loss;

            var fakeGameRoundRecord = Substitute.For<IGameRoundRecordRepository>();

            var web = CreateTestSettingSite(services =>
            {
                var fakeService = Substitute.For<ICommandService<UpdateClientStatusCommand>>();

                _ = fakeService.ExecuteAsync(new UpdateClientStatusCommand(roundId, status)).Returns(ValueTask.CompletedTask);
                _ = services.AddSingleton(fakeService);

                _ = services.AddSingleton(fakeGameRoundRecord);
            });

            var httpClient = web.CreateClient();

            var sut = new GameRoundsClient(httpClient);

            await sut.UpdateClientStatusAsync(roundId, status).ConfigureAwait(false);
        }
    }
}
