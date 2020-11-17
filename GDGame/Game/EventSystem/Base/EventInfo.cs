using GDGame.Enums;

namespace GDGame.EventSystem
 {
     public abstract class EventInfo
     {
     }
     
     public class PlayerEventInfo : EventInfo
     {
         public PlayerEventType type;

         public PlayerEventInfo()
         {
         }
     }
     
     public class GameStateMessageEventInfo : EventInfo
     {
         public GameState gameState;

         public GameStateMessageEventInfo(GameState gameState)
         {
             this.gameState = gameState;
         }
     }

     public class SceneEventInfo : EventInfo
     {
         public SceneActionType sceneActionType;
         public string LevelName { get; set; }
     }

     public class DataManagerEvent : EventInfo
     {
         
     }
 }