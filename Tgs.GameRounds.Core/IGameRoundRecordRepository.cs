namespace Tgs.GameRounds.Core
{
    public interface IGameRoundRecordRepository
    {
        public ValueTask AddAsync(GameRoundInfo gameRound);
    }
}
