using GDGame.Enums;
using GDGame.EventSystem;

namespace GDGame.Managers
{
    /// <summary>
    ///     Class that keeps track of the player's progress and moves and saves said progress for the levels.
    /// </summary>
    public class LevelDataManager
    {
        #region Private variables

        private int currentMovesCount;

        #endregion

        #region Constructors

        public LevelDataManager()
        {
            EventManager.RegisterListener<PlayerEventInfo>(HandlePlayerEvent);
            currentMovesCount = 0;
        }

        #endregion

        #region Events

        private void HandlePlayerEvent(PlayerEventInfo playerEventInfo)
        {
            switch (playerEventInfo.type)
            {
                case PlayerEventType.Move:
                    EventManager.FireEvent(new DataManagerEvent {CurrentMovesCount = ++currentMovesCount});
                    break;
            }
        }

        #endregion
    }
}