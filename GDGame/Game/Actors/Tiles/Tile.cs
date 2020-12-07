using System;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Game.Parameters.Effect;
using GDGame.Tiles;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    /// <summary>
    ///     Tile is the BasicTile from which other Tiles inherit
    /// </summary>
    public class Tile : OurCollidableObject
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

        #region Private variables

        private Vector3 spawnPos;

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

        public bool IsDirty { get; set; }

        public Shape Shape { get; set; }
        public ETileType TileType { get; }

        #endregion

        #region Initialization

        public void InitializeCollision(Vector3 position, float factor = 1f)
        {
            SetTranslation(position);
            AddPrimitive(new Box(Transform3D.Translation, Matrix.Identity, Transform3D.Scale * factor),
                new MaterialProperties(0.3f, 0.5f, 0.3f));
            Enable(true, 1);
        }

        public virtual void InitializeTile()
        {
            spawnPos = Transform3D.Translation;
            EventManager.RegisterListener<TileEventInfo>(HandleTileEvent);
        }

        #endregion

        #region Override Methode

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
            return HashCode.Combine(base.GetHashCode(), activatorId, spawnPos, Shape, (int) TileType);
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            Tile tile = new Tile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters, Model, IsBlocking, TileType);
            tile.ControllerList.AddRange(GetControllerListClone());
            return tile;
        }

        protected bool Equals(Tile other)
        {
            return base.Equals(other) && activatorId == other.activatorId && spawnPos.Equals(other.spawnPos) &&
                   Equals(Shape, other.Shape) && TileType == other.TileType;
        }

        public virtual void Respawn()
        {
            SetTranslation(spawnPos);
        }

        public void SetTranslation(Vector3 translation)
        {
            Transform3D.Translation = translation;
            Body.MoveTo(translation, Matrix.Identity);
            Body.ApplyGravity = false;
            Body.Immovable = true;
            Body.SetInactive();
            IsDirty = true;
        }

        #endregion

        #region Events

        private void HandleTileEvent(TileEventInfo info)
        {
            switch (info.Type)
            {
                case TileEventType.Reset:
                    if (info.Id == ID)
                    {
                        if (info.IsEasy)
                        {
                            Respawn();
                        }
                        else
                        {
                            EventManager.FireEvent(new GameStateMessageEventInfo {GameState = GameState.Lost});
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}