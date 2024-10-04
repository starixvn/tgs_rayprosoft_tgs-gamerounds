using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.IntegrationTest;

public partial class GameRoundsClientUnitTest
{
	[Fact]
	public async Task Round_Command()
	{
		var externalRoundId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var source = new RoundCommandViewModel
		{
			Id = Guid.NewGuid(),
			GameProviderId = Guid.NewGuid(),
			GameProviderConfigurationId = Guid.NewGuid(),
			GameId = 1,
			BrandId = Guid.NewGuid(),
			PlayerCode = "test",
			WalletId = Guid.NewGuid(),
			CurrencyCode = "CNY",
			Status = Common.Enums.RoundStatus.Closed,
			ClientStatus = Common.Enums.RoundClientStatus.Win,
			ExternalRoundId = externalRoundId,
			ExternalCreatedOn = DateTimeOffset.UtcNow.AddSeconds(-5),
			ExternalClosedOn = DateTimeOffset.UtcNow,
			BetAmount = 10.0m,
			ValidBetAmount = 10.0m,
			SettleAmount = 20.0m,
			Commission = 0.0m,
			PlayerWinLoss = 10.0m,
			CreatedOn = new DateTimeOffset(2024, 09, 16, 11, 36, 57, TimeSpan.FromHours(+08))
		};

		var web = new GameRoundsApplicationUnitTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeMediator = Substitute.For<IMediator>();

			_ = fakeMediator.Send(
				request: Arg.Is<RoundCommandRequest>(compare => compare.Id == source.Id &&
					compare.CreatedOn == source.CreatedOn &&
					compare.GameProviderId == source.GameProviderId &&
					compare.GameProviderConfigurationId == source.GameProviderConfigurationId &&
					compare.GameId == source.GameId &&
					compare.BrandId == source.BrandId &&
					compare.PlayerCode == source.PlayerCode &&
					compare.WalletId == source.WalletId &&
					compare.CurrencyCode == source.CurrencyCode &&
					compare.Status == source.Status &&
					compare.ClientStatus == source.ClientStatus &&
					compare.ExternalRoundId == source.ExternalRoundId &&
					compare.ExternalCreatedOn == source.ExternalCreatedOn &&
					compare.ExternalClosedOn == source.ExternalClosedOn &&
					compare.Bet == source.BetAmount &&
					compare.ValidBet == source.ValidBetAmount &&
					compare.Settle == source.SettleAmount &&
					compare.Commission == source.Commission &&
					compare.PlayerWinLoss == source.PlayerWinLoss),
				cancellationToken:
				Arg.Any<CancellationToken>())
			.Returns(Task.FromResult(source.Id));

			_ = services.AddTransient(_ => fakeMediator);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new GameRoundsClient(httpClient);

		var actual = await client.RoundCommandAsync(source);

		Assert.Equal(source.Id, actual);
	}
}
