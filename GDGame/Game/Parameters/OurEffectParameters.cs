using System;
using System.Collections.Generic;
using GDLibrary.Actors;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Parameters.Effect
{
    /// <summary>
    ///     Encapsulates the effect, texture, color (diffuse etc ) and alpha fields for any drawn 3D object.
    /// </summary>
    /// <see cref="GDLibrary.Actors.ModelObject" />
    /// <seealso cref="GDLibrary.Actors.PrimitiveObject" />
    public abstract class OurEffectParameters : ICloneable
    {
        #region Private variables

        //shader reference
        protected Microsoft.Xna.Framework.Graphics.Effect effect;

        #endregion

        #region Constructors

        //objects with texture and alpha but no specular or emmissive
        /// <summary>
        ///     Constructor for EffectParameters object used by any DrawnActor3D
        /// </summary>
        /// <param name="effect">Basic effect</param>
        /// <param name="texture">2D Texture</param>
        /// <param name="diffusecolor">RGBA diffuse color</param>
        /// <param name="alpha">Floating-point tansparency value</param>
        public OurEffectParameters(Microsoft.Xna.Framework.Graphics.Effect effect)
        {
            Effect = effect;
        }

        #endregion

        #region Properties, Indexers

        /// <summary>
        ///     Represents the Effect (i.e. shader code) used to render the drawn object
        /// </summary>
        /// <value>
        ///     Effect gets/sets the value of the effect field
        /// </value>
        public Microsoft.Xna.Framework.Graphics.Effect Effect
        {
            get => effect;
            set => effect = value;
        }

        #endregion

        #region Override Methode

        public override bool Equals(object obj)
        {
            return obj is OurEffectParameters parameters &&
                   EqualityComparer<Microsoft.Xna.Framework.Graphics.Effect>.Default.Equals(effect, parameters.effect);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(effect);
        }

        #endregion

        #region Methods

        public abstract object Clone();

        public virtual void DrawMesh(Matrix world, Camera3D camera3D, Model model, Matrix[] boneTransforms, GameTime gameTime)
        {
            SetupMaterial(world, camera3D, gameTime);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts) part.Effect = effect;

                effect.Parameters["World"].SetValue(boneTransforms[mesh.ParentBone.Index] * world);
                effect.Parameters["View"].SetValue(camera3D.View);
                effect.Parameters["Projection"].SetValue(camera3D.Projection);

                mesh.Draw();
            }
        }

        public virtual void DrawPrimitive(Matrix world, Camera3D camera, GameTime gameTime)
        {
            SetupMaterial(world, camera, gameTime);
            effect.CurrentTechnique.Passes[0].Apply();
        }

        public abstract float GetAlpha();

        public T GetTyped<T>() where T : OurEffectParameters
        {
            return (T) this;
        }

        protected abstract void SetupMaterial(Matrix world, Camera3D camera3D, GameTime gameTime);

        #endregion
    }

    public class BasicEffectParameters : OurEffectParameters
    {
        #region Private variables

        private BasicEffect basicEffect;
        private Color emissiveColor;

        #endregion

        #region Constructors

        public BasicEffectParameters(BasicEffect effect, Texture2D t, Color c, float a) : base(effect)
        {
            Texture = t;
            Color = c;
            Alpha = a;
            emissiveColor = Color.Transparent;
            basicEffect = effect;
        }

        public BasicEffectParameters(BasicEffect effect, Texture2D t, Color c, float a, Color e) : base(effect)
        {
            Texture = t;
            Color = c;
            Alpha = a;
            emissiveColor = e;
            basicEffect = effect;
        }

        #endregion

        #region Properties, Indexers

        public float Alpha { get; set; }

        public Color Color { get; set; }

        public Texture2D Texture { get; set; }

        #endregion

        #region Override Methode

        public override object Clone()
        {
            return new BasicEffectParameters(basicEffect, Texture, Color, Alpha, emissiveColor);
        }

        public override void DrawMesh(Matrix world, Camera3D camera3D, Model model, Matrix[] boneTransforms, GameTime gameTime)
        {
            SetupMaterial(world, camera3D, gameTime);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts) part.Effect = effect;
                basicEffect.World = boneTransforms[mesh.ParentBone.Index] * world;
                mesh.Draw();
            }
        }

        public override void DrawPrimitive(Matrix world, Camera3D camera, GameTime gameTime)
        {
            SetupMaterial(world, camera, gameTime);
        }

        public override float GetAlpha()
        {
            return Alpha;
        }

        protected override void SetupMaterial(Matrix world, Camera3D camera3D, GameTime gameTime)
        {
            basicEffect.Alpha = Alpha;
            if (Texture != null) basicEffect.Texture = Texture;
            basicEffect.EmissiveColor = emissiveColor.ToVector3();
            basicEffect.View = camera3D.View;
            basicEffect.Projection = camera3D.Projection;
            basicEffect.World = world;
            basicEffect.CurrentTechnique.Passes[0].Apply();
        }

        #endregion
    }

    public class NormalEffectParameters : OurEffectParameters
    {
        #region Private variables

        protected Texture2D colorTexture, normalTexture, displacement;
        protected Color diffuseColor;
        protected float diffuseIntensity;
        protected Transform3D lightTransform;

        #endregion

        #region Constructors

        public NormalEffectParameters(Microsoft.Xna.Framework.Graphics.Effect effect, Texture2D colorTexture, Texture2D normalTexture, Texture2D displacement, Color diffuseColor,
            float diffuseIntensity, Transform3D lightTransform) : base(effect)
        {
            this.effect = effect;
            this.colorTexture = colorTexture;
            this.normalTexture = normalTexture;
            this.diffuseColor = diffuseColor;
            this.diffuseIntensity = diffuseIntensity;
            this.lightTransform = lightTransform;
            this.displacement = displacement;
        }

        #endregion

        #region Properties, Indexers

        public Texture2D ColorTexture
        {
            get => colorTexture;
            set => colorTexture = value;
        }

        public Texture2D NormalTexture
        {
            get => normalTexture;
            set => normalTexture = value;
        }

        #endregion

        #region Override Methode

        public override object Clone()
        {
            return new NormalEffectParameters(effect, colorTexture, normalTexture, displacement, diffuseColor, diffuseIntensity, lightTransform);
        }

        public override float GetAlpha()
        {
            return 1;
        }

        protected override void SetupMaterial(Matrix world, Camera3D camera3D, GameTime gameTime)
        {
            effect.Parameters["DiffuseColor"].SetValue(diffuseColor.ToVector4());
            effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);
            effect.Parameters["Light"].SetValue(Vector3.Normalize(lightTransform.Look));
            effect.Parameters["ColorMap"].SetValue(colorTexture);
            effect.Parameters["NormalMap"].SetValue(normalTexture);
        }

        #endregion
    }

    public class CoffeeEffectParameters : OurEffectParameters
    {
        #region Private variables

        protected Color coffeeColor;
        protected Texture2D uvTilesTexture, flowTexture;
        protected float phase;

        #endregion

        public Color CoffeeColor
        {
            get => coffeeColor;
            set => coffeeColor = value;
        }

        public float Phase
        {
            get => phase;
            set => phase = value;
        }

        public Texture2D UvTilesTexture
        {
            get => uvTilesTexture;
            set => uvTilesTexture = value;
        }

        public Texture2D FlowTexture
        {
            get => flowTexture;
            set => flowTexture = value;
        }

        #region Constructors

        public CoffeeEffectParameters(Microsoft.Xna.Framework.Graphics.Effect effect, Texture2D uvTilesTexture, Texture2D flowTexture, Color coffeeColor) : base(effect)
        {
            phase = 0f;
            this.uvTilesTexture = uvTilesTexture;
            this.flowTexture = flowTexture;
            this.coffeeColor = coffeeColor;
            this.effect = effect;
        }

        #endregion

        #region Override Methode

        public override object Clone()
        {
            return new CoffeeEffectParameters(effect, uvTilesTexture, flowTexture, coffeeColor);
        }

        public override float GetAlpha()
        {
            return coffeeColor.ToVector4().W;
        }

        protected override void SetupMaterial(Matrix world, Camera3D camera3D, GameTime gameTime)
        {
            effect.Parameters["Displacement"].SetValue(uvTilesTexture);
            effect.Parameters["FlowMap"].SetValue(flowTexture);
            effect.Parameters["coffeeColor"].SetValue(coffeeColor.ToVector4());
            effect.Parameters["time"].SetValue(((float) gameTime.TotalGameTime.TotalSeconds) + phase);
        }

        #endregion
    }

    public class CelEffectParameters : OurEffectParameters
    {
        #region Private variables

        protected float alpha;

        protected Color color;

        //Not currently used
        protected Texture2D texture2D;

        #endregion

        #region Constructors

        public CelEffectParameters(Microsoft.Xna.Framework.Graphics.Effect effect, Texture2D t, Color c, float a) : base(effect)
        {
            texture2D = t;
            color = c;
            alpha = a;
        }

        #endregion

        #region Override Methode

        public override object Clone()
        {
            return new CelEffectParameters(effect, texture2D, color, alpha);
        }

        public override float GetAlpha()
        {
            return alpha;
        }

        protected override void SetupMaterial(Matrix world, Camera3D camera3D, GameTime gameTime)
        {
            effect.Parameters["WorldViewProjection"].SetValue(world * camera3D.View * camera3D.Projection);
            Effect.CurrentTechnique.Passes[0].Apply();
        }

        #endregion
    }
}