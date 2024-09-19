using Microsoft.EntityFrameworkCore;
using Tgs.GameRounds.Common.Enums;

using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Entities;

namespace Tgs.GameRounds.Repositories;

internal class RoundRepository(
	TgsContext context) : IRoundRepository
{
	public async ValueTask AddAsync(RoundAdd source, CancellationToken cancellationToken = default)
	{
		_ = await context.Rounds.AddAsync(
			entity: new Round()
			{
				Id = source.Id,
				ExternalRoundId = source.ExternalRoundId,
				GameProviderId = source.GameProviderId,
				PlayerCode = source.PlayerCode,
				GameId = source.GameId,
				CurrencyCode = source.CurrencyCode,
				Status = (short)source.Status,
				GameProviderConfigurationId = source.GameProviderConfigurationId,
				WalletId = source.WalletId,
				CreatedOn = source.CreatedOn.UtcDateTime,
				ClosedOn = source.ExternalClosedOn.HasValue ? DateTime.UtcNow : null,
				UpdatedOn = DateTime.UtcNow,
				BrandId = source.BrandId,
				ExternalCreatedOn = DateTime.SpecifyKind(source.ExternalCreatedOn.UtcDateTime, DateTimeKind.Utc),
				ExternalClosedOn = source.ExternalClosedOn.HasValue ? DateTime.SpecifyKind(source.ExternalClosedOn.Value.UtcDateTime, DateTimeKind.Utc) : null,
				BetAmount = source.Bet,
				ValidBetAmount = source.ValidBet,
				SettleAmount = source.Settle,
				Commission = source.ExternalClosedOn.HasValue ? source.Commission : 0,
				Turnover = source.Turnover,
				WinLoss = source.ExternalClosedOn.HasValue ? -source.PlayerWinLoss : 0,
				ClientStatus = (short)source.ClientStatus,
			},
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask ClosedAsync(DateTimeOffset externalCreatedOn, Guid id, RoundClosed source, CancellationToken cancellationToken = default)
	{
		var round = await context.Rounds.SingleAsync(
			predicate: x => x.CreatedOn >= externalCreatedOn.AddDays(-7).UtcDateTime &&
				x.Id == id &&
				x.Status == (short)RoundStatus.Open,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		round.Status = (short)RoundStatus.Closed;
		round.ClientStatus = (short)source.ClientStatus;
		round.ClosedOn = DateTime.UtcNow;
		round.UpdatedOn = DateTime.UtcNow;
		round.ExternalClosedOn = DateTime.SpecifyKind(source.ExternalClosedOn.UtcDateTime, DateTimeKind.Utc);
		round.BetAmount = source.Bet;
		round.ValidBetAmount = source.ValidBet;
		round.SettleAmount = source.Settle;
		round.Commission = source.Commission;
		round.Turnover = source.Turnover;
		round.WinLoss = -source.PlayerWinLoss;

		_ = context.Rounds.Update(round);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<RoundInfo?> GetAsync(DateTimeOffset externalCreatedOn, Guid id, CancellationToken cancellationToken = default)
		=> await context.Rounds
		.Where(r => r.CreatedOn >= externalCreatedOn.AddDays(-7).UtcDateTime && r.Id == id)
		.Select(round => new RoundInfo(
			round.Id,
			round.GameProviderId,
			round.GameProviderConfigurationId,
			round.GameId,
			round.BrandId,
			round.PlayerCode,
			round.WalletId,
			round.CurrencyCode,
			(RoundStatus)round.Status,
			(RoundClientStatus)round.ClientStatus,
			round.ExternalRoundId,
			round.ExternalCreatedOn,
			round.ExternalClosedOn,
			round.BetAmount.GetValueOrDefault(0),
			round.ValidBetAmount.GetValueOrDefault(0),
			round.SettleAmount.GetValueOrDefault(0),
			round.Commission.GetValueOrDefault(0),
			round.Turnover.GetValueOrDefault(0),
			-round.WinLoss.GetValueOrDefault(0),
			round.CreatedOn,
			round.ClosedOn,
			round.UpdatedOn))
		.FirstOrDefaultAsync(cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<RoundInfo?> GetAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, CancellationToken cancellationToken = default)
		=> await context.Rounds
		.Where(r => r.CreatedOn >= externalCreatedOn.AddDays(-7).UtcDateTime &&
			r.GameProviderId == gameProviderId &&
			r.ExternalRoundId == externalRoundId &&
			r.GameId == gameId &&
			r.WalletId == walletId)
		.Select(round => new RoundInfo(
			round.Id,
			round.GameProviderId,
			round.GameProviderConfigurationId,
			round.GameId,
			round.BrandId,
			round.PlayerCode,
			round.WalletId,
			round.CurrencyCode,
			(RoundStatus)round.Status,
			(RoundClientStatus)round.ClientStatus,
			round.ExternalRoundId,
			round.ExternalCreatedOn,
			round.ExternalClosedOn,
			round.BetAmount.GetValueOrDefault(0),
			round.ValidBetAmount.GetValueOrDefault(0),
			round.SettleAmount.GetValueOrDefault(0),
			round.Commission.GetValueOrDefault(0),
			round.Turnover.GetValueOrDefault(0),
			-round.WinLoss.GetValueOrDefault(0),
			round.CreatedOn,
			round.ClosedOn,
			round.UpdatedOn))
		.FirstOrDefaultAsync(cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<Guid?> GetIdAsync(DateTimeOffset externalCreatedOn, Guid gameProviderId, int gameId, Guid walletId, string externalRoundId, CancellationToken cancellationToken = default)
		=> await context.Rounds.AsQueryable()
		.Where(x =>
		x.CreatedOn >= externalCreatedOn.AddDays(-7).UtcDateTime &&
		x.CreatedOn <= DateTimeOffset.UtcNow &&
		x.ExternalRoundId == externalRoundId &&
		x.GameProviderId == gameProviderId &&
		x.GameId == gameId &&
		x.WalletId == walletId)
		.Select(x => (Guid?)x.Id)
		.FirstOrDefaultAsync(cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<RoundDetailInfo?> GetRoundDetailAsync(Guid roundId, CancellationToken cancellationToken = default)
		=> await context.RoundDetails
		.Where(x => x.RoundId == roundId)
		.Select(x => new RoundDetailInfo(x.RoundId, x.AdditionalData))
		.FirstOrDefaultAsync(cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask MergeDetailAsync(Guid id, string additionalData, CancellationToken cancellationToken = default)
	{
		var roundDetail = await context.RoundDetails
			.FirstOrDefaultAsync(
			predicate: x => x.RoundId == id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (roundDetail is null)
		{
			roundDetail = new RoundDetail()
			{
				RoundId = id,
				AdditionalData = additionalData,
			};

			_ = await context.RoundDetails.AddAsync(
				entity: roundDetail,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		else
		{
			roundDetail.AdditionalData = additionalData;

			_ = context.RoundDetails.Update(roundDetail);
		}

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask OpenAsync(DateTimeOffset externalCreatedOn, Guid id, RoundOpen source, CancellationToken cancellationToken = default)
	{
		var round = await context.Rounds.SingleAsync(
			predicate: x => x.CreatedOn >= externalCreatedOn.AddDays(-7).UtcDateTime &&
				x.Id == id &&
				x.Status == (short)RoundStatus.Open,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		round.Status = (short)RoundStatus.Open;
		round.ClientStatus = (short)RoundClientStatus.Open;
		round.UpdatedOn = DateTime.UtcNow;
		round.BetAmount = source.Bet;
		round.ValidBetAmount = source.ValidBet;
		round.SettleAmount = 0.0m;
		round.Commission = 0.0m;
		round.Turnover = source.Turnover;
		round.WinLoss = 0.0m;

		_ = context.Rounds.Update(round);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask ReopenAsync(DateTimeOffset externalCreatedOn, Guid id, RoundReopen source, CancellationToken cancellationToken = default)
	{
		var round = await context.Rounds.SingleAsync(
			predicate: x => x.CreatedOn >= externalCreatedOn.AddDays(-7).UtcDateTime &&
				x.Id == id &&
				x.Status == (short)RoundStatus.Closed,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		round.Status = (short)RoundStatus.Closed;
		round.ClientStatus = (short)source.ClientStatus;
		round.UpdatedOn = DateTime.UtcNow;
		round.ExternalClosedOn = DateTime.SpecifyKind(source.ExternalClosedOn.UtcDateTime, DateTimeKind.Utc);
		round.BetAmount = source.Bet;
		round.ValidBetAmount = source.ValidBet;
		round.SettleAmount = source.Settle;
		round.Commission = source.Commission;
		round.Turnover = source.Turnover;
		round.WinLoss = -source.PlayerWinLoss;
		round.ReopenTurnoverAdjustment = round.Turnover ?? 0;
		round.ReopenedOn = DateTime.UtcNow;

		_ = context.Rounds.Update(round);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
