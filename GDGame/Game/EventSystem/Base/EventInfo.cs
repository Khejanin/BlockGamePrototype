using GDGame.Actors;
using GDGame.Enums;
using Microsoft.Xna.Framework;

namespace GDGame.EventSystem
 {
     public abstract class EventInfo
     {
     }
     
     public class PlayerEventInfo : EventInfo
     {
         public PlayerEventType type;
         public Vector3? position;
         public AttachableTile attachedTile;

         public PlayerEventInfo()
         {
         }
     }

     public class ActivatorEventInfo : EventInfo
     {
         public ActivatorEventType type;
         public int id;

         public ActivatorEventInfo()
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