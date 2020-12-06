using System;
using System.Collections.Generic;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Utilities;

namespace GDGame.Managers
{
    public class LevelDataManager
    {
        #region Private variables

        private string currentLevel;

        private int currentTime;

        #endregion

        #region Constructors

        public LevelDataManager()
        {
            LevelStats = new Dictionary<string, LevelStats>();
            EventManager.RegisterListener<PlayerEventInfo>(HandlePlayerEvent);
            EventManager.RegisterListener<SceneEventInfo>(HandleSceneEvent);
            CurrentMovesCount = 0;
            currentTime = 0;
            currentLevel = "";
        }

        #endregion

        #region Properties, Indexers

        public int CurrentMovesCount { get; private set; }

        private Dictionary<string, LevelStats> LevelStats { get; }

        #endregion

        #region Methods

        private void HandelSceneEventInfoOnSceneChange()
        {
            if (!LevelStats.ContainsKey(currentLevel)) LevelStats.Add(currentLevel, new LevelStats());

            LevelStats levelStat = LevelStats[currentLevel];
            if (levelStat.MoveCount > CurrentMovesCount) levelStat.MoveCount = CurrentMovesCount;

            if (levelStat.Time > currentTime) levelStat.Time = currentTime;

            LevelStats[currentLevel] = levelStat;
        }

        private void HandelSceneEventInfoOnSceneLoaded(string levelName)
        {
            currentTime = 0;
            CurrentMovesCount = 0;
            currentLevel = levelName;
        }

        #endregion

        #region Events

        private void HandlePlayerEvent(PlayerEventInfo playerEventInfo)
        {
            switch (playerEventInfo.type)
            {
                case PlayerEventType.Move:
                    CurrentMovesCount++;
                    EventManager.FireEvent(new DataManagerEvent());
                    break;
                case PlayerEventType.Die:
                    break;
            }
        }

        private void HandleSceneEvent(SceneEventInfo sceneEventInfo)
        {
            switch (sceneEventInfo.SceneActionType)
            {
                case SceneActionType.OnSceneChange:
                    HandelSceneEventInfoOnSceneChange();
                    break;
                case SceneActionType.OnSceneLoaded:
                    HandelSceneEventInfoOnSceneLoaded(sceneEventInfo.LevelName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}