namespace Tgs.GameRounds.Common;

[Flags]
public enum GameRoundClientStatus
{
    Open = 1,
    Void = 2,
    Win = 3,
    Loss = 4,
    Draw = 5
}
