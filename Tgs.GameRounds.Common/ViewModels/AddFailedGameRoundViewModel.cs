namespace Tgs.GameRounds.Common.ViewModels;
public record AddFailedGameRoundViewModel(
   Guid GameProviderId,
   string GameProviderCode,
   string ExternalRoundId,
   string PlayerCode,
   int GameId,
   string ExternalGameId,
   string Detail);
