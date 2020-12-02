using System;
using System.Collections.Generic;
using GDGame.Enums;
using GDGame.EventSystem;

namespace GDGame.Managers
{
    public class LevelDataManager
    {
        private int currentMovesCount;

        public int CurrentMovesCount => currentMovesCount;

        private int currentTime;
        private string currentLevel;
        private Dictionary<string, LevelStats> LevelStats { get; }

        public LevelDataManager()
        {
            LevelStats = new Dictionary<string, LevelStats>();
            EventManager.RegisterListener<PlayerEventInfo>(HandlePlayerEvent);
            EventManager.RegisterListener<SceneEventInfo>(HandleSceneEvent);
            currentMovesCount = 0;
            currentTime = 0;
            currentLevel = "";
        }

        private void HandleSceneEvent(SceneEventInfo sceneEventInfo)
        {
            switch (sceneEventInfo.sceneActionType)
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

        private void HandelSceneEventInfoOnSceneLoaded(string levelName)
        {
            currentTime = 0;
            currentMovesCount = 0;
            currentLevel = levelName;
        }

        private void HandelSceneEventInfoOnSceneChange()
        {
            if (!LevelStats.ContainsKey(currentLevel))
            {
                LevelStats.Add(currentLevel, new LevelStats());
            }

            LevelStats levelStat = LevelStats[currentLevel];
            if (levelStat.moveCount > currentMovesCount)
            {
                levelStat.moveCount = currentMovesCount;
            }

            if (levelStat.time > currentTime)
            {
                levelStat.time = currentTime;
            }

            LevelStats[currentLevel] = levelStat;
        }

        private void HandlePlayerEvent(PlayerEventInfo playerEventInfo)
        {
            switch (playerEventInfo.type)
            {
                case PlayerEventType.Move:
                     currentMovesCount++;
                     EventManager.FireEvent(new DataManagerEvent());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    
}