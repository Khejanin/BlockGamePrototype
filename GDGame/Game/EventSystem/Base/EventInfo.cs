﻿using GDGame.Actors;
using GDGame.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.EventSystem
{
    public abstract class EventInfo
    {
    }

    public class TileEventInfo : EventInfo
    {
        #region Properties, Indexers

        public TileEventType Type { get; set; }
        public bool IsEasy { get; set; }

        #endregion
    }

    public class PlayerEventInfo : EventInfo
    {
        #region Public variables

        public AttachableTile movableTile;
        public Vector3? position;
        public Tile tile;
        public PlayerEventType type;

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
        public Transform3D transform;

        #endregion
    }

    public class OptionsEventInfo : EventInfo
    {
        #region Properties, Indexers

        public string Id { get; set; }
        public OptionsType Type { get; set; }

        #endregion
    }
}