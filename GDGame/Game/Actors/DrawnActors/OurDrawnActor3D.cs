using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Actors;

namespace GDGame.Actors
{ 
    /// <summary>
    /// Base class for all drawn 3D actors used in the engine. This class adds a EffectParameters field.
    /// </summary>
    public class OurDrawnActor3D : Actor3D, I3DDrawable
    {
        #region Fields

        private OurEffectParameters effectParameters;

        #endregion Fields

        #region Properties

        public OurEffectParameters EffectParameters
        {
            get
            {
                return effectParameters;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public OurDrawnActor3D(string id, ActorType actorType, StatusType statusType, Transform3D transform3D,
            OurEffectParameters effectParameters) : base(id, actorType, statusType, transform3D)
        {
            this.effectParameters = effectParameters;
        }

        public virtual void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
            //does nothing - see child classes
        }

        public override bool Equals(object obj)
        {
            return obj is OurDrawnActor3D d &&
                   base.Equals(obj) &&
                   EqualityComparer<OurEffectParameters>.Default.Equals(effectParameters, d.EffectParameters);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), effectParameters);
        }

        public new object Clone()
        {
            return new OurDrawnActor3D(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                effectParameters.Clone() as OurEffectParameters);
        }

        #endregion Constructors & Core
    }
}