using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.Models
{
    public record AddRound(
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
        decimal? BetAmount,
        decimal? ValidBetAmount,
        decimal? SettleAmount = null,
        decimal? Commission = null,
        decimal? Turnover = null,
        decimal? WinLoss = null,
        decimal? TieBetAdjustment = null,
        GameRoundClientStatus ClientStatus = GameRoundClientStatus.Open,
        string? AdditionalData = null);
}
