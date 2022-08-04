namespace Tgs.GameRounds.Core.ApplicationServices;

public record AddGameRoundRecordCommand(
    Guid RoundId,
    DateTimeOffset SendOn,
    DateTimeOffset ReceiveOn);
