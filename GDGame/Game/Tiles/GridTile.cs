using System;
using System.Collections.Generic;
using System.Threading;
using GDGame.Enums;
using GDGame.Game.Parameters.Effect;
using GDGame.Tiles;
using GDLibrary.Actors;
using GDLibrary.Containers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Tiles
{
    public class GridTile : ModelObject, ICloneable
    {
        private ETileType tileType;
        private Shape shape;

        public Shape Shape { get; set; }
        public ETileType TileType { get; set; }
        public bool IsMoving { get; set; }
        public Vector3 RotatePoint { get; set; }
        public int CurrentMovementTime { get; set; }

        public GridTile(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
        }

        public virtual void SetPosition(Vector3 position)
        {
            Transform3D.Translation = position;
        }

        public new object Clone()
        {
            GridTile gridTile = new GridTile("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as EffectParameters, Model);
            if (ControllerList != null)
            {
                foreach (IController controller in ControllerList)
                {
                    gridTile.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return gridTile;
        }
    }
}