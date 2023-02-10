using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.HandleServices;
public record GameRoundCheckingQuery(
    string ExternalRoundId,
    Guid GameProviderId,
    int GameId,
    string PlayerCode,
    Guid WalletId,
    GameRoundStatus Status,
    GameRoundClientStatus ClientStatus,
    DateTimeOffset ExternalCreatedOn,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal SettleAmount,
    decimal Commission,
    decimal Turnover,
    decimal PlayerWinLoss);
