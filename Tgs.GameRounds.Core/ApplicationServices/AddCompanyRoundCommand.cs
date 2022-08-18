using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.ApplicationServices
{
    public record AddCompanyRoundCommand(
        string ExternalRoundId,
        Guid GameProviderId,
        string PlayerCode,
        int GameId,
        string CurrencyCode,
        Guid GameProviderConfigurationId,
        Guid WalletId,
        Guid BrandId,
        DateTimeOffset ExternalCreatedOn,
        DateTimeOffset ExternalClosedOn,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal SettleAmount,
        decimal Commission,
        decimal Turnover,
        decimal CompanyWinLoss,
        GameRoundClientStatus ClientStatus,
        Guid OperatorId,
        string Remark);
}
