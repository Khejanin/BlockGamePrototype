using System;
using GDGame.Actors;
using GDGame.Enums;
using GDGame.Utilities;
using Microsoft.Xna.Framework;

namespace GDGame.EventSystem
{
    public abstract class EventInfo
    {
    }

    public class TileEventInfo : EventInfo
    {
        #region 04. Public variables

        public ETileType targetedTileType;
        public TileEventType type;

        #endregion
    }

    public class PlayerEventInfo : EventInfo
    {
        #region 04. Public variables

        public AttachableTile attachedTile;
        public Vector3? position;
        public PlayerEventType type;

        #endregion
    }

    public class ActivatorEventInfo : EventInfo
    {
        #region 04. Public variables

        public int id;
        public ActivatorEventType type;

        #endregion
    }

    public class GameStateMessageEventInfo : EventInfo
    {
        #region 04. Public variables

        public GameState gameState;

        #endregion

        #region 06. Constructors

        public GameStateMessageEventInfo(GameState gameState)
        {
            this.gameState = gameState;
        }

        #endregion
    }

    public class SceneEventInfo : EventInfo
    {
        #region 04. Public variables

        public SceneActionType sceneActionType;

        #endregion

        #region 07. Properties, Indexers

        public string LevelName { get; set; }

        #endregion
    }

    public class DataManagerEvent : EventInfo
    {
    }

    public class CameraEvent : EventInfo
    {
    }

    public class MovementEvent : EventInfo
    {
        #region 04. Public variables

        public Vector3 direction;
        public Action<Raycaster.HitResult> onCollideCallback;
        public Action onMoveEnd;
        public MovableTile tile;
        public MovementType type;

        #endregion
    }
}