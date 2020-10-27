using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//triangle = PrimitiveObject(vertex data) => DrawnActor3D(EffectParameters)
namespace GDLibrary
{
    public class DrawnActor3D : Actor3D
    {
        #region Fields
        private EffectParameters effectParameters;
        private RasterizerState rasterizerState; 
        #endregion

        #region Properties
        public EffectParameters EffectParameters
        {
            get
            {
                return this.effectParameters;
            }
        }

        public RasterizerState RasterizerState
        {
            get => rasterizerState;
            set => rasterizerState = value;
        }

        #endregion

        #region Constructors
        public DrawnActor3D(string id, ActorType actorType, StatusType statusType, Transform3D transform3D,
            EffectParameters effectParameters, RasterizerState rasterizerState = null) : base(id, actorType, statusType, transform3D)
        {
            this.rasterizerState = rasterizerState;
            this.effectParameters = effectParameters;
        }
        #endregion

        public override bool Equals(object obj)
        {
            return obj is DrawnActor3D d &&
                   base.Equals(obj) &&
                   EqualityComparer<EffectParameters>.Default.Equals(effectParameters, d.EffectParameters);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), effectParameters);
        }

        public new object Clone()
        {
            return new DrawnActor3D(this.ID, this.ActorType, this.StatusType, this.Transform3D.Clone() as Transform3D,
                this.effectParameters.Clone() as EffectParameters);
        }

        public virtual void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
            if (rasterizerState != null) graphicsDevice.RasterizerState = rasterizerState;
        }
    }
}
