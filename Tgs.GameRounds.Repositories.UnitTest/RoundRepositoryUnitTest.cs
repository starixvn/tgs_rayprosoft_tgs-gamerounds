using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tgs.GameRounds.Common.Enums;

using Tgs.GameRounds.Entities;

namespace Tgs.GameRounds.Repositories.UnitTest;

public class RoundRepositoryUnitTest
{
	[Fact]
	public async Task Get_Round()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: "bet.Round.Get_Round")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var roundId = new Guid("cc4d6693-417d-401a-8ccf-68f25382a134");
		var round = new Round
		{
			Id = roundId,
			ExternalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134",
			GameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
			PlayerCode = "4ues8xikza7o",
			GameId = 500001,
			CurrencyCode = "CNY",
			Status = (short)RoundStatus.Closed,
			GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			WalletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
			CreatedOn = DateTime.UtcNow,
			ClosedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			ExternalCreatedOn = DateTime.UtcNow,
			ExternalClosedOn = DateTime.UtcNow,
			BetAmount = 10m,
			ValidBetAmount = 10m,
			SettleAmount = 0.0m,
			Commission = 0.0m,
			Turnover = 10m,
			WinLoss = 10m,
			TieBetAdjustment = null,
			ClientStatus = (short)RoundClientStatus.Loss,
			ReopenTurnoverAdjustment = null,
			ReopenedOn = null,
			EffectiveBettingPercentage = null,
		};
		_ = await context.Rounds.AddAsync(round);
		_ = await context.RoundDetails.AddAsync(new RoundDetail()
		{
			RoundId = roundId,
			AdditionalData = JsonSerializer.Serialize(new { RoundId = roundId })
		});
		_ = await context.SaveChangesAsync();

		var fakeRoundRepository = new RoundRepository(context);

		var actual = await fakeRoundRepository.GetAsync(
			id: roundId,
			externalCreatedOn: DateTimeOffset.UtcNow);

		Assert.NotNull(actual);
		Assert.Equal(roundId, actual.Id);
		Assert.Equal(round.GameProviderId, actual.GameProviderId);
		Assert.Equal(round.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(round.GameId, actual.GameId);
		Assert.Equal(round.BrandId, actual.BrandId);
		Assert.Equal(round.PlayerCode, actual.PlayerCode);
		Assert.Equal(round.WalletId, actual.WalletId);
		Assert.Equal(round.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(round.Status, (int)actual.Status);
		Assert.Equal(round.ClientStatus, (int)actual.ClientStatus);
		Assert.Equal(round.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(round.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(round.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(round.BetAmount, actual.Bet);
		Assert.Equal(round.ValidBetAmount, actual.ValidBet);
		Assert.Equal(round.SettleAmount, actual.Settle);
		Assert.Equal(round.Commission, actual.Commission);
		Assert.Equal(round.Turnover, actual.Turnover);
		Assert.Equal(-round.WinLoss, actual.PlayerWinLoss);
		Assert.Equal(round.CreatedOn, actual.CreatedOn);
		Assert.Equal(round.ClosedOn, actual.ClosedOn);
		Assert.Equal(round.UpdatedOn, actual.UpdatedOn);
	}

	[Fact]
	public async Task Get_Round_ExternalId()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: "bet.Round.Get_Round_ExternalId")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var roundId = Guid.NewGuid();
		var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
		var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
		var gameId = 500001;
		var playerCode = "4ues8xikza7o";
		var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

		var round = new Round
		{
			Id = roundId,
			ExternalRoundId = externalRoundId,
			GameProviderId = gameProviderId,
			PlayerCode = playerCode,
			GameId = gameId,
			CurrencyCode = "CNY",
			Status = (short)RoundStatus.Closed,
			GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			WalletId = walletId,
			CreatedOn = DateTime.UtcNow,
			ClosedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			ExternalCreatedOn = DateTime.UtcNow,
			ExternalClosedOn = DateTime.UtcNow,
			BetAmount = 10m,
			ValidBetAmount = 10m,
			SettleAmount = 0.0m,
			Commission = 0.0m,
			Turnover = 10m,
			WinLoss = 10m,
			TieBetAdjustment = null,
			ClientStatus = (short)RoundClientStatus.Loss,
			ReopenTurnoverAdjustment = null,
			ReopenedOn = null,
			EffectiveBettingPercentage = null,
		};
		_ = await context.Rounds.AddAsync(round);
		_ = await context.RoundDetails.AddAsync(new RoundDetail()
		{
			RoundId = roundId,
			AdditionalData = JsonSerializer.Serialize(new { RoundId = roundId })
		});
		_ = await context.SaveChangesAsync();

		var fakeRoundRepository = new RoundRepository(context);

		var actual = await fakeRoundRepository.GetAsync(
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalRoundId: externalRoundId,
			externalCreatedOn: DateTimeOffset.UtcNow);

		Assert.NotNull(actual);
		Assert.Equal(roundId, actual.Id);
		Assert.Equal(round.GameProviderId, actual.GameProviderId);
		Assert.Equal(round.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(round.GameId, actual.GameId);
		Assert.Equal(round.BrandId, actual.BrandId);
		Assert.Equal(round.PlayerCode, actual.PlayerCode);
		Assert.Equal(round.WalletId, actual.WalletId);
		Assert.Equal(round.CurrencyCode, actual.CurrencyCode);
		Assert.Equal(round.Status, (int)actual.Status);
		Assert.Equal(round.ClientStatus, (int)actual.ClientStatus);
		Assert.Equal(round.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(round.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(round.ExternalClosedOn, actual.ExternalClosedOn);
		Assert.Equal(round.BetAmount, actual.Bet);
		Assert.Equal(round.ValidBetAmount, actual.ValidBet);
		Assert.Equal(round.SettleAmount, actual.Settle);
		Assert.Equal(round.Commission, actual.Commission);
		Assert.Equal(round.Turnover, actual.Turnover);
		Assert.Equal(-round.WinLoss, actual.PlayerWinLoss);
		Assert.Equal(round.CreatedOn, actual.CreatedOn);
		Assert.Equal(round.ClosedOn, actual.ClosedOn);
		Assert.Equal(round.UpdatedOn, actual.UpdatedOn);
	}

	[Fact]
	public async Task Get_Id()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: "bet.Round.Get_Id")
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var roundId = Guid.NewGuid();
		var gameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f");
		var externalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134";
		var gameId = 500001;
		var playerCode = "4ues8xikza7o";
		var walletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b");

		var round = new Round
		{
			Id = roundId,
			ExternalRoundId = externalRoundId,
			GameProviderId = gameProviderId,
			PlayerCode = playerCode,
			GameId = gameId,
			CurrencyCode = "CNY",
			Status = (short)RoundStatus.Closed,
			GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			WalletId = walletId,
			CreatedOn = DateTime.UtcNow,
			ClosedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			ExternalCreatedOn = DateTime.UtcNow,
			ExternalClosedOn = DateTime.UtcNow,
			BetAmount = 10m,
			ValidBetAmount = 10m,
			SettleAmount = 0.0m,
			Commission = 0.0m,
			Turnover = 10m,
			WinLoss = 10m,
			TieBetAdjustment = null,
			ClientStatus = (short)RoundClientStatus.Loss,
			ReopenTurnoverAdjustment = null,
			ReopenedOn = null,
			EffectiveBettingPercentage = null,
		};
		_ = await context.Rounds.AddAsync(round);
		_ = await context.RoundDetails.AddAsync(new RoundDetail()
		{
			RoundId = roundId,
			AdditionalData = JsonSerializer.Serialize(new { RoundId = roundId })
		});
		_ = await context.SaveChangesAsync();

		var fakeRoundRepository = new RoundRepository(context);

		var actual = await fakeRoundRepository.GetIdAsync(
			gameProviderId: gameProviderId,
			gameId: gameId,
			walletId: walletId,
			externalRoundId: externalRoundId,
			externalCreatedOn: DateTimeOffset.UtcNow);

		Assert.NotNull(actual);
		Assert.Equal(roundId, actual);
	}
}
