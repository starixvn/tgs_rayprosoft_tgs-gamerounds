namespace Tgs.GameRounds.Core.Models
{
    public record UpdateRoundOpen(
        Guid RoundId,
        DateTimeOffset UpdatedOn,
        decimal? BetAmount,
        decimal? ValidBetAmount,
        decimal? Commission,
        decimal? Turnover,
        string AdditionalData);
}
