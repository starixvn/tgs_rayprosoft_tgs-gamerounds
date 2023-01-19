using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core.Models;

public record AddFailedGameRound(
    Guid GameProviderId,
    string GameProviderCode,
    string ExternalRoundId,
    string PlayerCode,
    int GameId,
    string ExternalGameId,
    string Detail,
    FailedGameRoundAutoStatus AutoStatus,
    FailedGameRoundProcesStatus ProcesStatus);
