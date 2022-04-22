namespace Tgs.GameRounds.Common.ViewModels
{
    public record NewRoundQueryViewModel(
        string ExternalRoundId,
        Guid GameProviderId,
        string PlayerCode,
        int GameId,
        string CurrencyCode,
        Guid GameProviderConfigurationId,
        Guid WalletId,
        Guid BrandId,
        DateTimeOffset ExternalCreatedOn);
}
