using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;

namespace Tgs.GameRounds.IntegrationTests;

public partial class GameRoundsClientUnitTest
{
	[TestMethod]
	public async Task GameRounds_UpdateStatus_ExternalRoundId_Not_Exist()
	{
		var roundId = Guid.NewGuid();

		var status = GameRoundStatus.Closed;

		var externalClosedOn = DateTimeOffset.UtcNow;

		var web = CreateTestSettingSite(services =>
		{
			var fakeGameRound = Substitute.For<IGameRound>();
			_ = fakeGameRound.ExistAsync(roundId).Returns(ValueTask.FromResult(false));
			_ = services.AddSingleton(fakeGameRound);

			var fakeGameRoundRecord = Substitute.For<IGameRoundRecord>();
			_ = services.AddSingleton(fakeGameRoundRecord);
		});

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.UpdateStatusAsync(roundId, status, externalClosedOn).ConfigureAwait(false);

		Assert.IsNotNull(actual);
		Assert.AreEqual(false, actual.Success);
		Assert.AreEqual(HttpStatusCode.InternalServerError, actual.HttpStatusCode);
	}

	[TestMethod]
	public async Task GameRounds_UpdateStatus_Normal_Process()
	{
		var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
		var roundId = Guid.NewGuid();

		var status = GameRoundStatus.Closed;

		var externalClosedOn = DateTimeOffset.UtcNow;

		var fakeGameRoundRecordMessage = Substitute.For<IGameRoundRecordMessage>();

		var web = CreateTestSettingSite(services =>
		{
			var fakeGameRound = Substitute.For<IGameRound>();

			_ = fakeGameRound.ExistAsync(roundId).Returns(ValueTask.FromResult(true));

			_ = fakeGameRound.UpdateStatusAsync(roundId, status, externalClosedOn).Returns(ValueTask.CompletedTask);

			_ = services.AddSingleton(fakeGameRound);

			_ = services.AddSingleton(fakeGameRoundRecordMessage);
		});

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.UpdateStatusAsync(roundId, status, externalClosedOn).ConfigureAwait(false);

		Assert.IsNotNull(actual);
		Assert.AreEqual(true, actual.Success);
		Assert.AreEqual(HttpStatusCode.OK, actual.HttpStatusCode);

		_ = fakeGameRoundRecordMessage
			.Received(1)
			.SenderGameRoundRecord(Arg.Is(roundId));
	}
}
