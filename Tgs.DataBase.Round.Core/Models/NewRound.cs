namespace Tgs.DataBase.Round.Core.Models
{
    public record NewRound(
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
