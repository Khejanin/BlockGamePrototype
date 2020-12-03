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
    ///     Base class for all drawn 3D draw models objects used in the engine. This class adds a Model field.
    /// </summary>
    /// <see cref="GDLibrary.Actors.PrimitiveObject" />
    public class OurModelObject : OurDrawnActor3D
    {
        #region Private variables

        #endregion

        #region Constructors

        public OurModelObject(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, OurEffectParameters effectParameters, Model model)
            : base(id, actorType, statusType, transform, effectParameters)
        {
            this.Model = model;

            InitializeBoneTransforms();
        }

        #endregion

        #region Properties, Indexers

        public Matrix[] BoneTransforms { get; set; }

        public Model Model { get; set; }

        #endregion

        #region Initialization

        /// <summary>
        ///     3DS Max models contain meshes(e.g.a table might have 5 meshes i.e.a top and 4 legs) and each mesh contains a bone.
        ///     A bone holds the transform that says "move this mesh to this position".Without 5 bones in a table all the meshes
        ///     would collapse down to be centred on the origin.
        ///     Our table, wouldnt look very much like a table!
        ///     Take a look at the ObjectManager::DrawObject(GameTime gameTime, ModelObject modelObject) method and see if you can
        ///     figure out what the line below is doing:
        ///     effect.World = modelObject.BoneTransforms[mesh.ParentBone.Index] * modelObject.GetWorldMatrix();
        /// </summary>
        private void InitializeBoneTransforms()
        {
            //load bone transforms and copy transfroms to transform array (transforms)
            if (Model != null)
            {
                BoneTransforms = new Matrix[Model.Bones.Count];
                Model.CopyAbsoluteBoneTransformsTo(BoneTransforms);
            }
        }

        #endregion

        #region Override Methode

        public override void Draw(GameTime gameTime, Camera3D camera, GraphicsDevice graphicsDevice)
        {
            EffectParameters.DrawMesh(Transform3D.World, camera, Model, BoneTransforms, gameTime);
        }

        #endregion

        #region Methods

        public new object Clone()
        {
            OurModelObject actor = new OurModelObject("clone - " + ID, //deep
                ActorType, //deep
                StatusType,
                Transform3D.Clone() as Transform3D, //deep
                EffectParameters.Clone() as OurEffectParameters, //hybrid - shallow (texture and effect) and deep (all other fields)
                Model); //shallow i.e. a reference

            //remember if we clone a model then we need to clone any attached controllers
            if (ControllerList != null)
                //clone each of the (behavioural) controllers
                foreach (IController controller in ControllerList)
                    actor.ControllerList.Add(controller.Clone() as IController);

            return actor;
        }

        #endregion
    }
}