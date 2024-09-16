using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using Tgs.GameRounds.Common.Enums;
using Tgs.GameRounds.Core.Models;
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
			ClientStatus = (short)RoundClientStatus.Loss
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
			ClientStatus = (short)RoundClientStatus.Loss
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
			ClientStatus = (short)RoundClientStatus.Loss
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

	[Fact]
	public async Task Add_Status_Is_Closed()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: nameof(Add_Status_Is_Closed))
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var source = new RoundAdd(
			Id: new Guid("cc4d6693-417d-401a-8ccf-68f25382a134"),
			GameProviderId: new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: 500001,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: "4ues8xikza7o",
			WalletId: new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
			CurrencyCode: "CNY",
			Status: RoundStatus.Closed,
			ClientStatus: RoundClientStatus.Win,
			ExternalRoundId: "cc4d6693-417d-401a-8ccf-68f25382a134",
			ExternalCreatedOn: DateTime.UtcNow,
			ExternalClosedOn: DateTime.UtcNow,
			Bet: 10m,
			ValidBet: 10m,
			Settle: 20m,
			Commission: 0.0m,
			PlayerWinLoss: 10m,
			CreatedOn: DateTime.UtcNow
		);

		var fakeRoundRepository = new RoundRepository(context);

		await fakeRoundRepository.AddAsync(source: source);

		var actual = await context.Rounds
			.Where(r => r.CreatedOn == source.CreatedOn &&
			r.Id == source.Id)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(source.Id, actual.Id);
		Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(source.GameProviderId, actual.GameProviderId);
		Assert.Equal(source.PlayerCode, actual.PlayerCode);
		Assert.Equal(source.GameId, actual.GameId);
		Assert.Equal(source.CurrencyCode, actual.CurrencyCode);
		Assert.Equal((int)source.Status, (int)actual.Status);
		Assert.Equal(source.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(source.WalletId, actual.WalletId);
		Assert.Equal(source.CreatedOn, actual.CreatedOn);
		Assert.Equal(DateTime.UtcNow.ToShortDateString(), actual.ClosedOn!.Value.ToShortDateString());
		Assert.Equal(DateTime.UtcNow.ToShortDateString(), actual.UpdatedOn.ToShortDateString());
		Assert.Equal(source.BrandId, actual.BrandId);
		Assert.Equal(source.ExternalCreatedOn.UtcDateTime, actual.ExternalCreatedOn);
		Assert.Equal(source.ExternalClosedOn!.Value.UtcDateTime, actual.ExternalClosedOn!.Value);
		Assert.Equal(source.Bet, actual.BetAmount);
		Assert.Equal(source.ValidBet, actual.ValidBetAmount);
		Assert.Equal(source.Settle, actual.SettleAmount);
		Assert.Equal(source.Commission, actual.Commission);
		Assert.Equal(source.ValidBet, actual.Turnover);
		Assert.Equal(-source.PlayerWinLoss, actual.WinLoss);
		Assert.Equal((int)source.ClientStatus, (int)actual.ClientStatus);
	}

	[Fact]
	public async Task Add_Status_Is_Open()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: nameof(Add_Status_Is_Open))
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var source = new RoundAdd(
			Id: new Guid("cc4d6693-417d-401a-8ccf-68f25382a134"),
			GameProviderId: new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
			GameProviderConfigurationId: new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			GameId: 500001,
			BrandId: new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: "4ues8xikza7o",
			WalletId: new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
			CurrencyCode: "CNY",
			Status: RoundStatus.Open,
			ClientStatus: RoundClientStatus.Open,
			ExternalRoundId: "cc4d6693-417d-401a-8ccf-68f25382a134",
			ExternalCreatedOn: DateTime.UtcNow,
			ExternalClosedOn: null,
			Bet: 10m,
			ValidBet: 10m,
			Settle: 0.0m,
			Commission: 0.0m,
			PlayerWinLoss: 0.0m,
			CreatedOn: DateTime.UtcNow
		);

		var fakeRoundRepository = new RoundRepository(context);

		await fakeRoundRepository.AddAsync(source: source);

		var actual = await context.Rounds
			.Where(r => r.CreatedOn == source.CreatedOn &&
			r.Id == source.Id)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(source.Id, actual.Id);
		Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(source.GameProviderId, actual.GameProviderId);
		Assert.Equal(source.PlayerCode, actual.PlayerCode);
		Assert.Equal(source.GameId, actual.GameId);
		Assert.Equal(source.CurrencyCode, actual.CurrencyCode);
		Assert.Equal((int)source.Status, (int)actual.Status);
		Assert.Equal(source.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(source.WalletId, actual.WalletId);
		Assert.Equal(source.CreatedOn, actual.CreatedOn);
		Assert.Null(actual.ClosedOn);
		Assert.Equal(DateTime.UtcNow.ToShortDateString(), actual.UpdatedOn.ToShortDateString());
		Assert.Equal(source.BrandId, actual.BrandId);
		Assert.Equal(source.ExternalCreatedOn.UtcDateTime, actual.ExternalCreatedOn);
		Assert.Null(actual.ExternalClosedOn);
		Assert.Equal(source.Bet, actual.BetAmount);
		Assert.Equal(source.ValidBet, actual.ValidBetAmount);
		Assert.Equal(source.Settle, actual.SettleAmount);
		Assert.Equal(source.Commission, actual.Commission);
		Assert.Equal(source.ValidBet, actual.Turnover);
		Assert.Equal(-source.PlayerWinLoss, actual.WinLoss);
		Assert.Equal((int)source.ClientStatus, (int)actual.ClientStatus);
	}

	[Fact]
	public async Task Closed()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: nameof(Closed))
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();
		var roundId = new Guid("cc4d6693-417d-401a-8ccf-68f25382a134");
		var externalCreatedOn = DateTime.UtcNow;
		var round = new Round
		{
			Id = roundId,
			ExternalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134",
			GameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
			PlayerCode = "4ues8xikza7o",
			GameId = 500001,
			CurrencyCode = "CNY",
			Status = (short)RoundStatus.Open,
			GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			WalletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
			CreatedOn = externalCreatedOn,
			ClosedOn = null,
			UpdatedOn = DateTime.UtcNow,
			BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			ExternalCreatedOn = externalCreatedOn,
			ExternalClosedOn = null,
			BetAmount = 10m,
			ValidBetAmount = 10m,
			SettleAmount = 0.0m,
			Commission = 0.0m,
			Turnover = 0.0m,
			WinLoss = 0.0m,
			ClientStatus = (short)RoundClientStatus.Open
		};
		_ = await context.Rounds.AddAsync(round);
		_ = await context.RoundDetails.AddAsync(new RoundDetail()
		{
			RoundId = roundId,
			AdditionalData = JsonSerializer.Serialize(new { RoundId = roundId })
		});
		_ = await context.SaveChangesAsync();

		var source = new RoundClosed(
			ClientStatus: RoundClientStatus.Win,
			ExternalClosedOn: DateTime.UtcNow,
			Bet: 10.0m,
			ValidBet: 10.0m,
			Settle: 20.0m,
			Commission: 0.0m,
			PlayerWinLoss: 10.0m
		);

		var fakeRoundRepository = new RoundRepository(context);

		await fakeRoundRepository.ClosedAsync(externalCreatedOn, roundId, source);

		var actual = await context.Rounds
			.Where(r => r.Id == round.Id)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(round.Id, actual.Id);
		Assert.Equal(round.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(round.GameProviderId, actual.GameProviderId);
		Assert.Equal(round.PlayerCode, actual.PlayerCode);
		Assert.Equal(round.GameId, actual.GameId);
		Assert.Equal(round.CurrencyCode, actual.CurrencyCode);
		Assert.Equal((int)RoundStatus.Closed, (int)actual.Status);
		Assert.Equal(round.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(round.WalletId, actual.WalletId);
		Assert.Equal(round.CreatedOn, actual.CreatedOn);
		Assert.Equal(DateTime.UtcNow.ToShortDateString(), actual.ClosedOn!.Value.ToShortDateString());
		Assert.Equal(DateTime.UtcNow.ToShortDateString(), actual.UpdatedOn.ToShortDateString());
		Assert.Equal(round.BrandId, actual.BrandId);
		Assert.Equal(round.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(source.ExternalClosedOn.UtcDateTime, actual.ExternalClosedOn);
		Assert.Equal(source.Bet, actual.BetAmount);
		Assert.Equal(source.ValidBet, actual.ValidBetAmount);
		Assert.Equal(source.Settle, actual.SettleAmount);
		Assert.Equal(source.Commission, actual.Commission);
		Assert.Equal(source.ValidBet, actual.Turnover);
		Assert.Equal(-source.PlayerWinLoss, actual.WinLoss);
		Assert.Equal((int)source.ClientStatus, (int)actual.ClientStatus);
	}

	[Fact]
	public async Task Open()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: nameof(Closed))
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var roundId = new Guid("cc4d6693-417d-401a-8ccf-68f25382a134");
		var externalCreatedOn = DateTime.UtcNow;
		var round = new Round
		{
			Id = roundId,
			ExternalRoundId = "cc4d6693-417d-401a-8ccf-68f25382a134",
			GameProviderId = new Guid("eadd378a-3bb9-6186-3815-563e220c464f"),
			PlayerCode = "4ues8xikza7o",
			GameId = 500001,
			CurrencyCode = "CNY",
			Status = (short)RoundStatus.Open,
			GameProviderConfigurationId = new Guid("be8b70a3-a8fa-ecef-bfad-7a3455b9ef0f"),
			WalletId = new Guid("e7b4251c-e715-ec11-bf22-8c554ae8ab6b"),
			CreatedOn = externalCreatedOn,
			ClosedOn = null,
			UpdatedOn = DateTime.UtcNow,
			BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			ExternalCreatedOn = externalCreatedOn,
			ExternalClosedOn = null,
			BetAmount = 10m,
			ValidBetAmount = 10m,
			SettleAmount = 0.0m,
			Commission = 0.0m,
			Turnover = 0.0m,
			WinLoss = 0.0m,
			ClientStatus = (short)RoundClientStatus.Open
		};
		_ = await context.Rounds.AddAsync(round);
		_ = await context.RoundDetails.AddAsync(new RoundDetail()
		{
			RoundId = roundId,
			AdditionalData = JsonSerializer.Serialize(new { RoundId = roundId })
		});
		_ = await context.SaveChangesAsync();

		var source = new RoundOpen(
			Bet: 10m,
			ValidBet: 10m);

		var fakeRoundRepository = new RoundRepository(context);

		await fakeRoundRepository.OpenAsync(externalCreatedOn, roundId, source);

		var actual = await context.Rounds
			.Where(r => r.Id == round.Id)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(round.Id, actual.Id);
		Assert.Equal(round.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(round.GameProviderId, actual.GameProviderId);
		Assert.Equal(round.PlayerCode, actual.PlayerCode);
		Assert.Equal(round.GameId, actual.GameId);
		Assert.Equal(round.CurrencyCode, actual.CurrencyCode);
		Assert.Equal((int)RoundStatus.Open, (int)actual.Status);
		Assert.Equal(round.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(round.WalletId, actual.WalletId);
		Assert.Equal(round.CreatedOn, actual.CreatedOn);
		Assert.Null(actual.ClosedOn);
		Assert.Equal(DateTime.UtcNow.ToShortDateString(), actual.UpdatedOn.ToShortDateString());
		Assert.Equal(round.BrandId, actual.BrandId);
		Assert.Equal(round.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Null(actual.ExternalClosedOn);
		Assert.Equal(source.Bet, actual.BetAmount);
		Assert.Equal(source.ValidBet, actual.ValidBetAmount);
		Assert.Equal(0.0m, actual.SettleAmount);
		Assert.Equal(0.0m, actual.Commission);
		Assert.Equal(0.0m, actual.Turnover);
		Assert.Equal(0.0m, actual.WinLoss);
		Assert.Equal((int)RoundClientStatus.Open, (int)actual.ClientStatus);
	}

	[Fact]
	public async Task Reopen()
	{
		var options = new DbContextOptionsBuilder<TgsContext>()
		   .UseInMemoryDatabase(databaseName: nameof(Reopen))
		   .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
		   .Options;
		var context = new TgsContext(options);
		_ = context.Database.EnsureDeleted();

		var roundId = new Guid("cc4d6693-417d-401a-8ccf-68f25382a134");
		var externalCreatedOn = DateTime.UtcNow;
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
			CreatedOn = externalCreatedOn,
			ClosedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow,
			BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
			ExternalCreatedOn = externalCreatedOn,
			ExternalClosedOn = DateTime.UtcNow,
			BetAmount = 10m,
			ValidBetAmount = 10m,
			SettleAmount = 20.0m,
			Commission = 0.0m,
			Turnover = 10m,
			WinLoss = -10m,
			ClientStatus = (short)RoundClientStatus.Win
		};
		_ = await context.Rounds.AddAsync(round);
		_ = await context.RoundDetails.AddAsync(new RoundDetail()
		{
			RoundId = roundId,
			AdditionalData = JsonSerializer.Serialize(new { RoundId = roundId })
		});
		_ = await context.SaveChangesAsync();

		var source = new RoundReopen(
			ClientStatus: RoundClientStatus.Loss,
			ExternalClosedOn: DateTime.UtcNow,
			Bet: 10m,
			ValidBet: 10m,
			Settle: 0.0m,
			Commission: 0.0m,
			PlayerWinLoss: -10m
		);

		var fakeRoundRepository = new RoundRepository(context);

		await fakeRoundRepository.ReopenAsync(externalCreatedOn, roundId, source);

		var actual = await context.Rounds
			.Where(r => r.Id == round.Id)
			.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(round.Id, actual.Id);
		Assert.Equal(round.ExternalRoundId, actual.ExternalRoundId);
		Assert.Equal(round.GameProviderId, actual.GameProviderId);
		Assert.Equal(round.PlayerCode, actual.PlayerCode);
		Assert.Equal(round.GameId, actual.GameId);
		Assert.Equal(round.CurrencyCode, actual.CurrencyCode);
		Assert.Equal((int)RoundStatus.Closed, (int)actual.Status);
		Assert.Equal(round.GameProviderConfigurationId, actual.GameProviderConfigurationId);
		Assert.Equal(round.WalletId, actual.WalletId);
		Assert.Equal(round.CreatedOn, actual.CreatedOn);
		Assert.Equal(round.ClosedOn, actual.ClosedOn!.Value);
		Assert.Equal(DateTime.UtcNow.ToShortDateString(), actual.UpdatedOn.ToShortDateString());
		Assert.Equal(round.BrandId, actual.BrandId);
		Assert.Equal(round.ExternalCreatedOn, actual.ExternalCreatedOn);
		Assert.Equal(source.ExternalClosedOn.UtcDateTime, actual.ExternalClosedOn);
		Assert.Equal(source.Bet, actual.BetAmount);
		Assert.Equal(source.ValidBet, actual.ValidBetAmount);
		Assert.Equal(source.Settle, actual.SettleAmount);
		Assert.Equal(source.Commission, actual.Commission);
		Assert.Equal(source.ValidBet, actual.Turnover);
		Assert.Equal(-source.PlayerWinLoss, actual.WinLoss);
		Assert.Equal((int)source.ClientStatus, (int)actual.ClientStatus);
		Assert.Equal(round.Turnover, actual.ReopenTurnoverAdjustment!.Value);
		Assert.Equal(DateTime.UtcNow.ToShortDateString(), actual.ReopenedOn!.Value.ToShortDateString());
	}
}
