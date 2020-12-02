namespace GDGame.Utilities
{
    public class LevelStats
    {
        #region Constructors

        public LevelStats()
        {
            Time = int.MaxValue;
            MoveCount = int.MaxValue;
        }

        #endregion

        #region Properties, Indexers

        public int MoveCount { get; set; }

        public int Time { get; set; }

        #endregion
    }
}