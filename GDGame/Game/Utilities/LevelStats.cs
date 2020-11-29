namespace GDGame.Utilities
{
    public class LevelStats
    {
        #region 06. Constructors

        public LevelStats()
        {
            Time = int.MaxValue;
            MoveCount = int.MaxValue;
        }

        #endregion

        #region 07. Properties, Indexers

        public int MoveCount { get; set; }

        public int Time { get; set; }

        #endregion
    }
}