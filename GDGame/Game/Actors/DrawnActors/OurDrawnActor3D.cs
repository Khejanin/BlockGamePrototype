using System;
using System.Collections.Generic;
using GDGame.Game.Parameters.Effect;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    /// <summary>
    /// Here the "Our" legacy begins for drawn Actors. Due to the custom EffectParameters we needed to have a custom implementation of the drawn Actors.
    /// </summary>
    public class OurDrawnActor3D : Actor3D, I3DDrawable
    {
        #region Private variables

        #endregion

        #region Constructors

        public OurDrawnActor3D(string id, ActorType actorType, StatusType statusType, Transform3D transform3D,
            OurEffectParameters effectParameters) : base(id, actorType, statusType, transform3D)
        {
            this.EffectParameters = effectParameters;
        }

        #endregion

        #region Properties, Indexers

        public OurEffectParameters EffectParameters { get; }

        #endregion

        #region Override Methode

        public override bool Equals(object obj)
        {
            return obj is OurDrawnActor3D d &&
                   base.Equals(obj) &&
                   EqualityComparer<OurEffectParameters>.Default.Equals(EffectParameters, d.EffectParameters);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), EffectParameters);
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            return new OurDrawnActor3D(ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters);
        }

        public virtual void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
            //does nothing - see child classes
        }

        #endregion
    }
}