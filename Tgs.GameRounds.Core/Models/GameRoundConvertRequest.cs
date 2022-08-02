namespace Tgs.GameRounds.Core.Models
{
    public record GameRoundConvertRequest(
        Guid GameProviderId,
        string CurrencyCode,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal SettleAmount,
        decimal Commission,
        decimal Turnover,
        decimal PlayerWinLoss);
}
