using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.Models
{
    public record AddGameRoundRecord(
        Guid RoundId,
        string ExternalRoundId,
        string ExternalGameId,
        Guid GameProviderId,
        string GameProviderCode,
        Guid GameProviderConfigurationId,
        int GameId,
        Guid BrandId,
        string PlayerCode,
        Guid WalletId,
        string CurrencyCode,
        GameRoundStatus Status,
        GameRoundClientStatus ClientStatus,
        DateTimeOffset CreatedOn,
        DateTimeOffset? ClosedOn,
        DateTimeOffset UpdatedOn,
        DateTimeOffset? ExternalCreatedOn,
        DateTimeOffset? ExternalClosedOn,
        decimal Bet,
        decimal ValidBet,
        decimal Settle,
        decimal Commission,
        decimal Turnover,
        decimal CompanyWinLoss,
        decimal TieBetAdjustment,
        decimal ReopenTurnoverAdjustment,
        DateTimeOffset? ReopenedOn,
        DateTimeOffset SendOn,
        DateTimeOffset ReceiveOn);
}
