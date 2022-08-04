namespace Tgs.GameRounds.MessageHandlers
{
    public record GameRoundRecordReceive(
        Guid RoundId,
        DateTimeOffset SendOn);
}
