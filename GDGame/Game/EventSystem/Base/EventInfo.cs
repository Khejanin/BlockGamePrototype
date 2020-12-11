using System;
using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Enums;
using GDLibrary.Actors;
using GDLibrary.Parameters;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.EventSystem
{
    /// <summary>
    /// Base EventInfo Class that all EventInformation needs to inherit from.
    /// Inheriting from EventInfo makes it possible for other classes to listen from events of exactly that type.
    /// </summary>
    public abstract class EventInfo
    {
    }

    public class TileEventInfo : EventInfo
    {
        #region Properties, Indexers
        
        public TileEventType Type { get; set; }
        public bool IsEasy { get; set; }

        public string Id { get; set; }

        #endregion
    }

    public class PlayerEventInfo : EventInfo
    {
        #region Public variables

        public AttachableTile movableTile;
        public Vector3? position;
        public PlayerEventType type;

        #endregion
    }

    public class RemoveActorEvent : EventInfo
    {
        public Body body;
        public Actor3D actor3D;
        public Actor2D actor2D;
    }

    public class ActivatorEventInfo : EventInfo
    {
        #region Public variables

        public int id;
        public ActivatorEventType type;

        #endregion
    }

    public class MovingTilesEventInfo : EventInfo
    {
    }

    public class GameStateMessageEventInfo : EventInfo
    {
        #region Public variables

        public GameState GameState { get; set; }

        #endregion
    }

    public class SceneEventInfo : EventInfo
    {
        #region Properties, Indexers

        public string LevelName { get; set; }
        public SceneActionType SceneActionType { get; set; }

        #endregion
    }

    public class DataManagerEvent : EventInfo
    {
    }

    public class MovementEvent : EventInfo
    {
        #region Public variables

        public Vector3 direction;
        public MovementType type;

        #endregion
    }

    public class SoundEventInfo : EventInfo
    {
        #region Public variables

        public SfxType sfxType;
        public SoundEventType soundEventType;
        public SoundVolumeType soundVolumeType;
        public SoundCategory category;
        public Vector3? soundLocation;
        public Transform3D listenerTransform;

        #endregion
    }

    public class OptionsEventInfo : EventInfo
    {
        #region Properties, Indexers

        public string Id { get; set; }
        public OptionsType Type { get; set; }

        #endregion
    }

    public enum CoffeeEventType
    {
        CoffeeStartMoving,
        CoffeeDanger,
        CoffeeDangerStop
    }

    public class CoffeeEventInfo : EventInfo
    {
        public CoffeeEventType coffeeEventType;
    }
}