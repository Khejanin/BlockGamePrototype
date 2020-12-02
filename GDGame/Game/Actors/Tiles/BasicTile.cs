using System;
using GDGame.Game.Parameters.Effect;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Tiles;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class BasicTile : OurCollidableObject
    {
        #region Enums

        public enum EStaticTileType
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

        public BasicTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, OurEffectParameters effectParameters, Model model, ETileType tileType)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            TileType = tileType;
        }

        #endregion

        #region Properties, Indexers

        public Shape Shape { get; set; }

        protected ETileType TileType { get; }

        #endregion

        #region Initialization

        public virtual void InitializeTile()
        {
            spawnPos = Transform3D.Translation;
            EventManager.RegisterListener<TileEventInfo>(HandleTileEvent);
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            BasicTile basicTile = new BasicTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D, EffectParameters.Clone() as OurEffectParameters, Model,
                TileType);
            basicTile.ControllerList.AddRange(GetControllerListClone());
            return basicTile;
        }

        public void Respawn()
        {
            Transform3D.Translation = spawnPos;
        }

        #endregion

        #region Events

        private void HandleTileEvent(TileEventInfo info)
        {
            if (info.targetedTileType != ETileType.None && info.targetedTileType != TileType)
                return;

            switch (info.type)
            {
                case TileEventType.Reset:
                    Respawn();
                    break;
            }
        }

        #endregion
    }
}