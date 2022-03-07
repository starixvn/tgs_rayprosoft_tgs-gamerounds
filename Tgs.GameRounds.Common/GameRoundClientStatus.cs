namespace Tgs.GameRounds.Common
{
    [Flags]
    public enum GameRoundClientStatus
    {
        Open = 1,
        Void,
        Win,
        Loss,
        Draw
    }
}
