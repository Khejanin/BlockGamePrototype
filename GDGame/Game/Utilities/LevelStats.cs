namespace GDGame.Utilities
{
    public class LevelStats
    {
        public int Time { get; set; }
        public int MoveCount { get; set; }

        public LevelStats()
        {
            Time = int.MaxValue;
            MoveCount = int.MaxValue;
        }
    }
}