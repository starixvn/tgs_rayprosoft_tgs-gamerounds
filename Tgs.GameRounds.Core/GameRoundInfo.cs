namespace Tgs.GameRounds.Core
{
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
        GameRoundClientStatus ClientStatus,
        decimal? ReopenTurnoverAdjustment,
        DateTime? ReopenedOn,
        decimal? EffectiveBettingPercentage,
        string AdditionalData
        );
}
