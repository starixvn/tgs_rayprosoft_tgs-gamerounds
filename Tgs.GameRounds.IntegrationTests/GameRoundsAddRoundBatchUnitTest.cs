using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.IntegrationTests;

public partial class GameRoundsClientUnitTest
{
	[TestMethod]
	public async Task GameRounds_AddRoundBatch_Normal_Process()
	{
		var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
		var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
		var gameId = 500001;
		var playerCode = "4ues8xikza7o";
		var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");
		var addRoundBatch = new AddRoundBatchViewModel(
			ExternalRoundId: externalRoundId,
			GameProviderId: gameProviderId,
			PlayerCode: playerCode,
			GameId: gameId,
			CurrencyCode: "CNY",
			Status: GameRoundStatus.Open,
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			WalletId: walletId,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			ExternalCreatedOn: DateTimeOffset.UtcNow,
			ExternalClosedOn: null,
			BetAmount: 10m,
			ValidBetAmount: 10m,
			SettleAmount: null,
			Commission: 0,
			Turnover: 10m,
			PlayerWinLoss: null,
			ClientStatus: GameRoundClientStatus.Open,
			AdditionalData: "{}");

		var model = new AddRoundBatchsViewModel()
		{
			AddRoundBatches = new List<AddRoundBatchViewModel>() { addRoundBatch }
		};

		var web = CreateTestSettingSite(services =>
		{
			var fakeService = Substitute.For<IGetService<AddRoundBatchCommand, IEnumerable<Guid>>>();

			_ = fakeService.GetAsync(Arg.Any<AddRoundBatchCommand>()).Returns(new List<Guid>() { Guid.NewGuid() });

			_ = services.AddSingleton(fakeService);
		});

		var httpClient = web.CreateClient();

		var sut = new GameRoundsClient(httpClient);

		var actual = await sut.AddRoundBatchAsync(model).ConfigureAwait(false);

		Assert.IsNotNull(actual);
		Assert.AreEqual(true, actual.Success);
		Assert.AreEqual(HttpStatusCode.OK, actual.HttpStatusCode);
		Assert.IsTrue(actual.RoundIds.Any());
	}
}
