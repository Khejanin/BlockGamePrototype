using System;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Game.Parameters.Effect;
using GDGame.Managers;
using GDGame.Tiles;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    /// <summary>
    ///     Tile is the BasicTile from which other Tiles inherit.
    /// </summary>
    public class Tile : OurCollidableObject, IDisposable
    {
        #region Enums

        public enum StaticTileType
        {
            Chocolate,
            DarkChocolate,
            WhiteChocolate,
            Plates
        }

        #endregion

        #region Public variables

        public int activatorId = -1;

        #endregion

        #region Constructors

        public Tile(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, bool isBlocking, ETileType tileType) : base(id,
            actorType, statusType, transform, effectParameters, model, isBlocking)
        {
            TileType = tileType;
        }

        #endregion

        #region Properties, Indexers

        //Tiles can be part of a shape -> a collection of Tiles
        public Shape Shape { get; set; }

        public Vector3 SpawnPos { get; protected set; }

        //All Tiles have a TileType
        public ETileType TileType { get; }

        #endregion

        #region Initialization

        public virtual void InitializeCollision(Vector3 position, float factor = 1f)
        {
            SetTranslation(position);
            AddPrimitive(new Box(Transform3D.Translation, Matrix.Identity, Transform3D.Scale * factor),
                new MaterialProperties(0.3f, 0.5f, 0.3f));
            Enable(true, 1);
        }

        //Called after Clone() so that we don't break the Game by setting this up in the constructor.
        public virtual void InitializeTile()
        {
            SpawnPos = Transform3D.Translation;
            EventManager.RegisterListener<TileEventInfo>(HandleTileEvent);
        }

        #endregion

        #region Override Method

        public override void Enable(bool bImmovable, float mass)
        {
            base.Enable(bImmovable, mass);
            Body.ApplyGravity = false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Tile) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), activatorId, SpawnPos, Shape, (int) TileType);
        }

        #endregion

        #region Public Method

        public new object Clone()
        {
            Tile tile = new Tile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters, Model, IsBlocking, TileType);
            tile.ControllerList.AddRange(GetControllerListClone());
            return tile;
        }

        public virtual void Respawn()
        {
            if (!Body.IsActive)
                Body.SetActive();
            SetTranslation(SpawnPos);
            Transform3D.RotationInDegrees = Vector3.Zero;
        }

        //Method which sets a Translation and does everything so it also works for the Body
        public virtual void SetTranslation(Vector3 translation)
        {
            Transform3D.Translation = translation;
            Body.MoveTo(translation, Matrix.Identity);
            Body.Immovable = true;
        }

        #endregion

        #region Private Method

        //Animation that is played when the Tile dies. It has a Callback that executes everything that the user needs after the dying animation (Respawn/Remove from ObjectManager).
        protected virtual void Die(Action callbackAfterDeath)
        {
            this.ScaleTo(new AnimationEventData
            {
                isRelative = false, destination = Vector3.Zero,
                maxTime = 1000,
                smoothing = Smoother.SmoothingMethod.Accelerate, loopMethod = LoopMethod.PlayOnce,
                callback = callbackAfterDeath, resetAferDone = true
            });
        }

        protected bool Equals(Tile other)
        {
            return base.Equals(other) && activatorId == other.activatorId && SpawnPos.Equals(other.SpawnPos) &&
                   Equals(Shape, other.Shape) && TileType == other.TileType;
        }

        #endregion

        #region Events

        private void HandleTileEvent(TileEventInfo info)
        {
            if (info.Id == ID)
                switch (info.Type)
                {
                    case TileEventType.Reset:
                        if (info.IsEasy)
                            Die(Respawn);
                        else
                            EventManager.FireEvent(new GameStateMessageEventInfo {GameState = GameState.Lost});
                        break;

                    case TileEventType.Consumed:
                        Die(() => { EventManager.FireEvent(new RemoveActorEvent {actor3D = this}); });
                        break;
                }
        }

        #endregion

        public virtual void Dispose()
        {
            EventManager.UnregisterListener<TileEventInfo>(HandleTileEvent);
        }
    }
}