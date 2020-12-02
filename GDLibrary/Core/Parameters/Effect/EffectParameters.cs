using System;
using System.Collections.Generic;
using GDGame.Scenes;
using GDLibrary.Actors;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Parameters.Effect
{
    /// <summary>
    /// Encapsulates the effect, texture, color (diffuse etc ) and alpha fields for any drawn 3D object.
    /// </summary>
    /// <see cref="GDLibrary.Actors.ModelObject"/>
    /// <seealso cref="GDLibrary.Actors.PrimitiveObject"/>
    public abstract class EffectParameters : ICloneable
    {
        #region Fields

        //shader reference
        protected Microsoft.Xna.Framework.Graphics.Effect effect;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Represents the Effect (i.e. shader code) used to render the drawn object
        /// </summary>
        /// <value>
        /// Effect gets/sets the value of the effect field
        /// </value>
        public Microsoft.Xna.Framework.Graphics.Effect Effect
        {
            get
            {
                return effect;
            }
            set
            {
                effect = value;
            }
        }

        #endregion Properties

        #region Constructors & Core

        //objects with texture and alpha but no specular or emmissive
        /// <summary>
        /// Constructor for EffectParameters object used by any DrawnActor3D
        /// </summary>
        /// <param name="effect">Basic effect</param>
        /// <param name="texture">2D Texture</param>
        /// <param name="diffusecolor">RGBA diffuse color</param>
        /// <param name="alpha">Floating-point tansparency value</param>
        public EffectParameters(Microsoft.Xna.Framework.Graphics.Effect effect)
        {
            Effect = effect;
        }

        protected abstract void SetupMaterial(Matrix world, Camera3D camera3D,GameTime gameTime);

        public virtual void DrawPrimitive(Matrix world, Camera3D camera,GameTime gameTime)
        {
            SetupMaterial(world,camera,gameTime);
            effect.CurrentTechnique.Passes[0].Apply();
        }

        public virtual void DrawMesh(Matrix world, Camera3D camera3D, Model model, Matrix[] boneTransforms,GameTime gameTime)
        {
            SetupMaterial(world,camera3D,gameTime);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
                
                effect.Parameters["World"].SetValue(boneTransforms[mesh.ParentBone.Index]*world);
                effect.Parameters["View"].SetValue(camera3D.View);
                effect.Parameters["Projection"].SetValue(camera3D.Projection);
                
                mesh.Draw();
            }
        }

        public T GetTyped<T>() where T : EffectParameters
        {
            return (T)this;
        }

        public abstract float GetAlpha();

        public abstract object Clone();

        public override bool Equals(object obj)
        {
            return obj is EffectParameters parameters &&
                   EqualityComparer<Microsoft.Xna.Framework.Graphics.Effect>.Default.Equals(effect, parameters.effect);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(effect);
        }

        #endregion Constructors & Core
    }

    public class BasicEffectParameters : EffectParameters
    {
        private Texture2D texture;
        private Color color;
        private float alpha;

        public Texture2D Texture
        {
            get => texture;
            set => texture = value;
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public float Alpha
        {
            get => alpha;
            set => alpha = value;
        }

        public BasicEffectParameters(BasicEffect effect,Texture2D t,Color c, float a) : base(effect)
        {
            texture = t;
            color = c;
            alpha = a;
        }

        protected override void SetupMaterial(Matrix world, Camera3D camera3D,GameTime gameTime)
        {
            BasicEffect basicEffect = (BasicEffect) effect;
            basicEffect.Alpha = alpha;
            basicEffect.Texture = texture;
            basicEffect.EmissiveColor = color.ToVector3();
            basicEffect.View = camera3D.View;
            basicEffect.Projection = camera3D.Projection;
            
            Effect.CurrentTechnique.Passes[0].Apply();
        }

        public override void DrawPrimitive(Matrix world, Camera3D camera,GameTime gameTime)
        {
            base.DrawPrimitive(world,camera, gameTime);
            effect.CurrentTechnique.Passes[0].Apply();
        }

        public override void DrawMesh(Matrix world, Camera3D camera3D, Model model, Matrix[] boneTransforms,GameTime gameTime)
        {
            base.DrawMesh(world,camera3D, model, boneTransforms, gameTime);
            
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
                ((BasicEffect)effect).World = boneTransforms[mesh.ParentBone.Index] * world;
                mesh.Draw();
            }
        }

        public override float GetAlpha()
        {
            return alpha;
        }

        public override object Clone()
        {
            return new BasicEffectParameters((BasicEffect)effect,texture,color,alpha);
        }
    }

    public class NormalEffectParameters : EffectParameters
    {
        protected  Texture2D colorTexture, normalTexture;
        protected Color ambientColor, diffuseColor;
        protected float ambientIntensity, diffuseIntensity;
        protected Transform3D lightTransform;
        
        public NormalEffectParameters(Microsoft.Xna.Framework.Graphics.Effect effect, Texture2D colorTexture, Texture2D normalTexture, Color diffuseColor, Color ambientColor, float ambientIntensity, float diffuseIntensity,Transform3D lightTransform) : base(effect)
        {
            this.effect = effect;
            this.colorTexture = colorTexture;
            this.normalTexture = normalTexture;
            this.diffuseColor = diffuseColor;
            this.ambientColor = ambientColor;
            this.ambientIntensity = ambientIntensity;
            this.diffuseIntensity = diffuseIntensity;
            this.lightTransform = lightTransform;
        }

        protected override void SetupMaterial(Matrix world, Camera3D camera3D,GameTime gameTime)
        {
            effect.Parameters["AmbientColor"].SetValue(Color.Black.ToVector4());
            effect.Parameters["AmbientIntensity"].SetValue(1f);
            effect.Parameters["DiffuseColor"].SetValue(Vector4.One);
            effect.Parameters["DiffuseIntensity"].SetValue(1f);
            effect.Parameters["Light"].SetValue(Vector3.Normalize(lightTransform.RotationInDegrees));
            effect.Parameters["ColorMap"].SetValue(colorTexture);
            effect.Parameters["NormalMap"].SetValue(normalTexture);
        }

        public override float GetAlpha()
        {
            return 1;
        }

        public override object Clone()
        {
            return new NormalEffectParameters(effect,colorTexture,normalTexture,diffuseColor,ambientColor,ambientIntensity,diffuseIntensity,lightTransform);
        }
    }
    
    public class CoffeeEffectParameters : EffectParameters
    {
        protected  Texture2D uvTilesTexture, flowTexture;
        protected Color coffeeColor;
        
        public CoffeeEffectParameters(Microsoft.Xna.Framework.Graphics.Effect effect, Texture2D uvTilesTexture,Texture2D flowTexture, Color coffeeColor) : base(effect)
        {
            this.uvTilesTexture = uvTilesTexture;
            this.flowTexture = flowTexture;
            this.coffeeColor = coffeeColor;
            this.effect = effect;
        }

        protected override void SetupMaterial(Matrix world, Camera3D camera3D,GameTime gameTime)
        {
            effect.Parameters["AmbientColor"].SetValue(Color.Black.ToVector4());
            effect.Parameters["AmbientIntensity"].SetValue(1f);
            effect.Parameters["DiffuseColor"].SetValue(Vector4.One);
            effect.Parameters["DiffuseIntensity"].SetValue(1f);
        }

        public override float GetAlpha()
        {
            return 1;
        }

        public override object Clone()
        {
            return new CoffeeEffectParameters(effect,uvTilesTexture,flowTexture,coffeeColor);
        }
    }
    
    public class CelEffectParameters : EffectParameters
    {
        //Not currently used
        protected Texture2D texture2D;
        protected Color color;
        protected float alpha;
        
        
        public CelEffectParameters(Microsoft.Xna.Framework.Graphics.Effect effect, Texture2D t, Color c,float a) : base(effect)
        {
            texture2D = t;
            color = c;
            alpha = a;
        }
        
        protected override void SetupMaterial(Matrix world, Camera3D camera3D,GameTime gameTime)
        {
            effect.Parameters["WorldViewProjection"].SetValue(world*camera3D.View*camera3D.Projection);
            Effect.CurrentTechnique.Passes[0].Apply();
        }

        public override float GetAlpha()
        {
            return alpha;
        }

        public override object Clone()
        {
            return new CelEffectParameters(effect,texture2D,color,alpha);
        }
    }
    
    public class TestEffectParameters : EffectParameters
    {
        //Not currently used
        protected Texture2D texture2D;
        protected Color color;
        protected float alpha;
        protected Transform3D playerPosition;

        public Transform3D PlayerPosition
        {
            get => playerPosition;
            set => playerPosition = value;
        }

        public TestEffectParameters(Microsoft.Xna.Framework.Graphics.Effect effect,Transform3D position, Texture2D t, Color c,float a) : base(effect)
        {
            texture2D = t;
            color = c;
            alpha = a;
            playerPosition = position;
        }
        
        protected override void SetupMaterial(Matrix world, Camera3D camera3D,GameTime gameTime)
        {
            effect.Parameters["WorldViewProjection"].SetValue(world*camera3D.View*camera3D.Projection);
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["PlayerPos"].SetValue(MainScene.playerTransform3D.Translation);
            Effect.CurrentTechnique.Passes[0].Apply();
        }

        public override float GetAlpha()
        {
            return alpha;
        }

        public override object Clone()
        {
            return new TestEffectParameters(effect,playerPosition,texture2D,color,alpha);
        }
    }
    
}