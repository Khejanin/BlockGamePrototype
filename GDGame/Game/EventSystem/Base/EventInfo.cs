using GDGame.Enums;

namespace GDGame.EventSystem
 {
     public abstract class EventInfo
     {
     }
     
     class PlayerEventInfo : EventInfo
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
 }