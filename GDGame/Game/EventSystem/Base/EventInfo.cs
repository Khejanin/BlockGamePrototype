using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Enums;
using GDLibrary.Actors;
using GDLibrary.Parameters;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace GDGame.EventSystem
{
    /// <summary>
    ///     Base EventInfo Class that all EventInformation needs to inherit from.
    ///     Inheriting from EventInfo makes it possible for other classes to listen from events of exactly that type.
    /// </summary>
    public abstract class EventInfo
    {
    }

    public class TileEventInfo : EventInfo
    {
        #region Properties, Indexers

        public string Id { get; set; }
        public bool IsEasy { get; set; }

        public TileEventType Type { get; set; }

        #endregion
    }

    public class PlayerEventInfo : EventInfo
    {
        #region Public variables

        public List<Vector3> checkpoints;

        public AttachableTile movableTile;
        public Vector3? position;
        public PlayerEventType type;

        #endregion
    }

    public class RemoveActorEvent : EventInfo
    {
        #region Public variables

        public Actor3D actor3D;
        public Body body;

        #endregion
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
        #region Properties, Indexers

        public GameState GameState { get; set; }

        #endregion
    }

    public class DataManagerEvent : EventInfo
    {
        #region Properties, Indexers

        public int CurrentMovesCount { get; set; }

        #endregion
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

        public SoundCategory category;
        public Transform3D listenerTransform;

        public SfxType sfxType;
        public SoundEventType soundEventType;
        public Vector3? soundLocation;
        public SoundVolumeType soundVolumeType;

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
        #region Public variables

        public CoffeeEventType coffeeEventType;

        #endregion
    }
}