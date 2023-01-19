namespace Tgs.GameRounds.Common;

[Flags]
public enum GameRoundStatus
{
    New = 0,
    Open = 1,
    Closed = 2,
    Void = 3,
}
