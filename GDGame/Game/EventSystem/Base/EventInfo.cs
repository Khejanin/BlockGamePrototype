using System;
using GDGame.Actors;
using GDGame.Enums;
using GDGame.Utilities;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.EventSystem
 {
     public abstract class EventInfo
     {
     }

     public class TileEventInfo : EventInfo
     {
         public ETileType targetedTileType;
         public TileEventType type;
     }
     
     public class PlayerEventInfo : EventInfo
     {
         public PlayerEventType type;
         public Vector3? position;
         public AttachableTile attachedTile;
         
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
     
     public class CameraEvent : EventInfo
     {
         
     }
     
     public class MovementEvent : EventInfo
     {
         public MovementType type;
         public Vector3 direction;
         public Action onMoveEnd;
         public Action<Raycaster.HitResult> onCollideCallback;
         public MovableTile tile;
     }
 }