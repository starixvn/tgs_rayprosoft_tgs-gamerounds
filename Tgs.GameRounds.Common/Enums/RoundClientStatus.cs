namespace Tgs.GameRounds.Common.Enums;

[Flags]
public enum RoundClientStatus
{
	New = 0,
	Open = 1,
	Void = 2,
	Win = 3,
	Loss = 4,
	Draw = 5,
}
