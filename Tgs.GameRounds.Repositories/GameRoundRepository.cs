using Microsoft.EntityFrameworkCore;

using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Repositories.Entities;

namespace Tgs.GameRounds.Repositories;

internal class GameRoundRepository(
    BetContext context) : IGameRoundRepository
{
    public async ValueTask<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Rounds.AsQueryable()
        .AnyAsync(
            predicate: x => x.Id == id,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false);

    public ValueTask<GameRoundInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Rounds.AsQueryable()
        .Where(x => x.Id == id)
        .Select(round => new GameRoundInfo(
            round.Id,
            round.ExternalRoundId,
            round.GameProviderId,
            round.PlayerCode,
            round.GameId,
            round.CurrencyCode,
            (GameRoundStatus)round.Status,
            round.GameProviderConfigurationId,
            round.WalletId,
            round.CreatedOn,
            round.ClosedOn,
            round.UpdatedOn,
            round.BrandId,
            round.ExternalCreatedOn,
            round.ExternalClosedOn,
            round.BetAmount.GetValueOrDefault(0),
            round.ValidBetAmount.GetValueOrDefault(0),
            round.SettleAmount.GetValueOrDefault(0),
            round.Commission.GetValueOrDefault(0),
            round.Turnover.GetValueOrDefault(0),
            round.WinLoss.GetValueOrDefault(0),
            round.TieBetAdjustment.GetValueOrDefault(0),
            (GameRoundClientStatus)round.ClientStatus,
            round.ReopenTurnoverAdjustment,
            round.ReopenedOn,
            round.EffectiveBettingPercentage))
        .ToAsyncEnumerable()
        .FirstOrDefaultAsync(cancellationToken);

    public ValueTask<GameRoundInfo?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default)
        => context.Rounds.AsQueryable()
        .Where(x => x.GameProviderId == gameProviderId &&
        x.ExternalRoundId == externalRoundId &&
        x.GameId == gameId &&
        x.WalletId == walletId)
        .Select(round => new GameRoundInfo(
            round.Id,
            round.ExternalRoundId,
            round.GameProviderId,
            round.PlayerCode,
            round.GameId,
            round.CurrencyCode,
            (GameRoundStatus)round.Status,
            round.GameProviderConfigurationId,
            round.WalletId,
            round.CreatedOn,
            round.ClosedOn,
            round.UpdatedOn,
            round.BrandId,
            round.ExternalCreatedOn,
            round.ExternalClosedOn,
            round.BetAmount.GetValueOrDefault(0),
            round.ValidBetAmount.GetValueOrDefault(0),
            round.SettleAmount.GetValueOrDefault(0),
            round.Commission.GetValueOrDefault(0),
            round.Turnover.GetValueOrDefault(0),
            round.WinLoss.GetValueOrDefault(0),
            round.TieBetAdjustment.GetValueOrDefault(0),
            (GameRoundClientStatus)round.ClientStatus,
            round.ReopenTurnoverAdjustment,
            round.ReopenedOn,
            round.EffectiveBettingPercentage))
        .ToAsyncEnumerable()
        .FirstOrDefaultAsync(cancellationToken);

    public async ValueTask<GameRoundDetailInfo?> GetRoundDetailAsync(Guid roundId, CancellationToken cancellationToken = default)
        => await context.RoundDetails
            .AsQueryable()
            .Where(x => x.RoundId == roundId)
            .Select(x => new GameRoundDetailInfo(x.RoundId, x.AdditionalData))
            .FirstOrDefaultAsync(cancellationToken);

    public ValueTask<Guid?> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default)
        => context.Rounds.AsQueryable()
        .Where(x => x.ExternalRoundId == externalRoundId
            && x.GameProviderId == gameProviderId
            && x.GameId == gameId
            && x.WalletId == walletId)
        .Select(x => (Guid?)x.Id)
        .ToAsyncEnumerable()
        .FirstOrDefaultAsync(cancellationToken);
}
