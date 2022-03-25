namespace Tgs.GameRounds.Common.ViewModels
{
    public record GameRoundViewModel(
            Guid RoundId,
            string ExternalRoundId,
            Guid GameProviderId,
            string GameProviderCode,
            string PlayerCode,
            int GameId,
            string ExternalGameId,
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
            decimal? SettleAmount,
            decimal? Commission,
            decimal? Turnover,
            decimal? WinLoss,
            decimal? TieBetAdjustment,
            GameRoundClientStatus ClientStatus,
            decimal? ReopenTurnoverAdjustment,
            DateTimeOffset? ReopenedOn,
            decimal? EffectiveBettingPercentage,
            string AdditionalData
            );
}
