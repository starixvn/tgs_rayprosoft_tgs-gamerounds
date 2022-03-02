namespace Tgs.GameRounds.Core
{
    public record RoundInfo(
        Guid RoundId,
        string ExternalRoundId,
        Guid GameProviderId,
        string PlayerCode,
        int GameId,
        string CurrencyCode,
        RoundStatus Status,
        Guid GameProviderConfigurationId,
        Guid WalletId,
        DateTime CreatedOn,
        DateTime? ClosedOn,
        DateTime UpdatedOn,
        Guid BrandId,
        DateTime? ExternalCreatedOn,
        DateTime? ExternalClosedOn,
        decimal? BetAmount,
        decimal? ValidBetAmount,
        decimal? SettleAmount,
        decimal? Commission,
        decimal? Turnover,
        decimal? WinLoss,
        decimal? TieBetAdjustment,
        RoundClientStatus ClientStatus,
        decimal? ReopenTurnoverAdjustment,
        DateTime? ReopenedOn,
        decimal? EffectiveBettingPercentage,
        string AdditionalData
        );
}
