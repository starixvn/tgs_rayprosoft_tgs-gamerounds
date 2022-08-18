using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Exceptions;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.IntegrationTests;

public partial class GameRoundsClientUnitTest
{
	[TestMethod]
	public async Task GameRounds_UpdateRoundOpen_ExternalRoundId_Not_Exist()
	{
		var roundId = Guid.NewGuid();

		var model = new UpdateRoundOpenViewModel()
		{
			BetAmount = 10m,
			ValidBetAmount = 10m,
			Commission = 0,
			Turnover = 10m
		};

		var web = CreateTestSettingSite(services =>
		{
			var fakeService = Substitute.For<ICommandService<UpdateRoundOpenCommand>>();

			_ = fakeService.ExecuteAsync(Arg.Any<UpdateRoundOpenCommand>()).Throws<GameRoundDoesNotExistException>();

			_ = services.AddSingleton(fakeService);
		});

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.UpdateRoundOpenAsync(roundId, model).ConfigureAwait(false);

		Assert.IsNotNull(actual);
		Assert.AreEqual(false, actual.Success);
		Assert.AreEqual(HttpStatusCode.InternalServerError, actual.HttpStatusCode);
	}

	[TestMethod]
	public async Task GameRounds_UpdateRoundOpen_Normal_Process()
	{
		var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
		var roundId = Guid.NewGuid();

		var model = new UpdateRoundOpenViewModel()
		{
			BetAmount = 10m,
			ValidBetAmount = 10m,
			Commission = 0,
			Turnover = 10m
		};

		var web = CreateTestSettingSite(services =>
		{
			var fakeService = Substitute.For<ICommandService<UpdateRoundOpenCommand>>();

			_ = fakeService.ExecuteAsync(Arg.Any<UpdateRoundOpenCommand>()).Returns(ValueTask.CompletedTask);

			_ = services.AddSingleton(fakeService);
		});

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.UpdateRoundOpenAsync(roundId, model).ConfigureAwait(false);

		Assert.IsNotNull(actual);
		Assert.AreEqual(true, actual.Success);
		Assert.AreEqual(HttpStatusCode.OK, actual.HttpStatusCode);
	}
}
