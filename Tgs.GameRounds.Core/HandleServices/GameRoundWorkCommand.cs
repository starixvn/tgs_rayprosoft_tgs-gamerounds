using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.HandleServices;
public record GameRoundWorkCommand(
    string ExternalRoundId,
    Guid GameProviderId,
    int GameId,
    string PlayerCode,
    string CurrencyCode,
    Guid WalletId,
    Guid BrandId,
    Guid GameProviderConfigurationId,
    GameRoundStatus Status,
    GameRoundClientStatus ClientStatus,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset? ExternalClosedOn,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal SettleAmount,
    decimal Commission,
    decimal Turnover,
    decimal PlayerWinLoss,
    string AdditionalData);
