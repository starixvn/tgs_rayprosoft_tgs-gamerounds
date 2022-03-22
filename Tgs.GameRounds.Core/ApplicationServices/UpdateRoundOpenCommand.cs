namespace Tgs.GameRounds.Core.ApplicationServices
{
    public record UpdateRoundOpenCommand(
        Guid RoundId,
        DateTimeOffset UpdatedOn,
        decimal? BetAmount,
        decimal? ValidBetAmount,
        decimal? Commission,
        decimal? Turnover,
        string AdditionalData);
}
