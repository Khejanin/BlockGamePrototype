using System;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Tiles;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    public class BasicTile : ModelObject, ICloneable
    {
        #region 02. Enums

        public enum EStaticTileType
        {
            Chocolate,
            DarkChocolate,
            WhiteChocolate,
            Plates
        }

        #endregion

        #region 04. Public variables

        public int activatorId = -1;

        #endregion

        #region 05. Private variables

        private Vector3 spawnPos;

        #endregion

        #region 06. Constructors

        public BasicTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model, ETileType tileType)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            TileType = tileType;
        }

        #endregion

        #region 07. Properties, Indexers

        public Shape Shape { get; set; }

        protected ETileType TileType { get; }

        #endregion

        #region 08. Initialization

        public virtual void InitializeTile()
        {
            spawnPos = Transform3D.Translation;
            EventManager.RegisterListener<TileEventInfo>(HandleTileEvent);
        }

        #endregion

        #region 11. Methods

        public new object Clone()
        {
            BasicTile basicTile = new BasicTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model, TileType);
            if (ControllerList != null)
                foreach (IController controller in ControllerList)
                    basicTile.ControllerList.Add(controller.Clone() as IController);

            return basicTile;
        }

        public void Respawn()
        {
            Transform3D.Translation = spawnPos;
        }

        #endregion

        #region 12. Events

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