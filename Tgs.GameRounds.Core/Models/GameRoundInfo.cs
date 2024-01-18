using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.Models;

public record GameRoundInfo(
    Guid RoundId,
    string ExternalRoundId,
    Guid GameProviderId,
    string PlayerCode,
    int GameId,
    string CurrencyCode,
    GameRoundStatus Status,
    Guid GameProviderConfigurationId,
    Guid WalletId,
    DateTimeOffset CreatedOn,
    DateTimeOffset? ClosedOn,
    DateTimeOffset UpdatedOn,
    Guid BrandId,
    DateTimeOffset? ExternalCreatedOn,
    DateTimeOffset? ExternalClosedOn,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal SettleAmount,
    decimal Commission,
    decimal Turnover,
    decimal CompanyWinLoss,
    decimal TieBetAdjustment,
    GameRoundClientStatus ClientStatus,
    decimal? ReopenTurnoverAdjustment,
    DateTimeOffset? ReopenedOn,
    decimal? EffectiveBettingPercentage);
