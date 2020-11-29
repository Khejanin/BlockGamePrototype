﻿using GDGame.Actors;
using GDGame.Enums;
using Microsoft.Xna.Framework;

namespace GDGame.EventSystem
 {
     public abstract class EventInfo
     {
     }

     public class TileEventInfo : EventInfo
     {
         public TileType targetedTileType;
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

     public class SoundEventInfo : EventInfo
     {
         public SoundEventType soundEventType;
         public SfxType sfxType;
         public SoundVolumeType soundVolumeType;
     }
 }