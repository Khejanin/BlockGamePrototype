using System;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using GDGame.Game.Parameters.Effect;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Base class for all drawn 3D draw models objects used in the engine. This class adds a Model field.
    /// </summary>
    /// <see cref="GDLibrary.Actors.PrimitiveObject"/>
    public class ModelObject : DrawnActor3D, ICloneable
    {
        #region Fields

        private Model model;
        private Matrix[] boneTransforms;

        #endregion Fields

        #region Properties

        public Model Model
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
            }
        }

        public Matrix[] BoneTransforms
        {
            get
            {
                return boneTransforms;
            }
            set
            {
                boneTransforms = value;
            }
        }

        #endregion Properties

        public ModelObject(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters)
        {
            this.model = model;

            InitializeBoneTransforms();
        }

        /// <summary>
        /// 3DS Max models contain meshes(e.g.a table might have 5 meshes i.e.a top and 4 legs) and each mesh contains a bone.
        ///
        /// A bone holds the transform that says "move this mesh to this position".Without 5 bones in a table all the meshes would collapse down to be centred on the origin.
        /// Our table, wouldnt look very much like a table!
        /// Take a look at the ObjectManager::DrawObject(GameTime gameTime, ModelObject modelObject) method and see if you can figure out what the line below is doing:
        ///   effect.World = modelObject.BoneTransforms[mesh.ParentBone.Index] * modelObject.GetWorldMatrix();
        /// </summary>
        private void InitializeBoneTransforms()
        {
            //load bone transforms and copy transfroms to transform array (transforms)
            if (model != null)
            {
                boneTransforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            }
        }

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
            EffectParameters.DrawMesh(Transform3D.World,camera,model,boneTransforms);
        }

        public new object Clone()
        {
            ModelObject actor = new ModelObject("clone - " + ID, //deep
               ActorType,   //deep
               StatusType,
               Transform3D.Clone() as Transform3D,  //deep
               EffectParameters.Clone() as EffectParameters, //hybrid - shallow (texture and effect) and deep (all other fields)
               model); //shallow i.e. a reference

            //remember if we clone a model then we need to clone any attached controllers
            if (ControllerList != null)
            {
                //clone each of the (behavioural) controllers
                foreach (IController controller in ControllerList)
                {
                    actor.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return actor;
        }

        #region OurCode

        public List<BoundingSphere> GetBounds()
        {
            List<BoundingSphere> result = new List<BoundingSphere>();
            
            foreach (ModelMesh mesh in Model.Meshes)
            {
                result.Add(mesh.BoundingSphere);
            }

            return result;
        }

        #endregion
    }
}