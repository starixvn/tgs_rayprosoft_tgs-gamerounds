namespace Tgs.GameRounds.Common.ViewModels
{
    public record UpdateRoundOpenViewModel(
        DateTimeOffset UpdatedOn,
        decimal? BetAmount,
        decimal? ValidBetAmount,
        decimal? Commission,
        decimal? Turnover,
        string AdditionalData);
}
