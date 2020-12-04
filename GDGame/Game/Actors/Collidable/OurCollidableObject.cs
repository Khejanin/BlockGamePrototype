using GDGame.Game.Parameters.Effect;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Actors
{
    /// <summary>
    ///     Allows us to draw a model and add a collision skin and physics response. Use this class to create collidable
    ///     objects
    ///     and add one or more of the 3 collision primitive types (i.e. Box, Sphere, Capsule). Unlike the TriangleMeshObject
    ///     this class will allow an object to MOVE within the game (e.g. fall, by lifted and dropped, be pushed etc).
    /// </summary>
    public class OurCollidableObject : OurModelObject
    {
        #region Private variables

        private Vector3 com;
        private Matrix it, itCoM;
        protected bool isBlocking = true;

        private float junk;

        #endregion

        public bool IsBlocking => isBlocking;

        #region Constructors

        public OurCollidableObject(string id, ActorType actorType, StatusType statusType, Transform3D transform, OurEffectParameters effectParameters, Model model,bool isBlocking)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            this.isBlocking = isBlocking;
            Body = new Body();
            Body.ExternalData = this;
            Collision = new CollisionSkin(Body);
            Body.CollisionSkin = Collision;

            //we will only add this event handling in a class that sub-classes CollidableObject e.g. PickupCollidableObject or PlayerCollidableObject
            //this.body.CollisionSkin.callbackFn += CollisionSkin_callbackFn;
        }

        #endregion

        #region Properties, Indexers

        public Body Body { get; set; }

        public CollisionSkin Collision { get; set; }

        public float Mass { get; set; }

        #endregion

        #region Override Methode

        //we will only add this method in a class that sub-classes CollidableObject e.g. PickupCollidableObject or PlayerCollidableObject
        //private bool CollisionSkin_callbackFn(CollisionSkin skin0, CollisionSkin skin1)
        //{
        //    return true;
        //}

        public override Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(Transform3D.Scale) *
                   Collision.GetPrimitiveLocal(0).Transform.Orientation *
                   Body.Orientation *
                   Transform3D.Orientation *
                   Matrix.CreateTranslation(Body.Position);
        }

        #endregion

        #region Methods

        public void AddPrimitive(Primitive primitive, MaterialProperties materialProperties)
        {
            Collision.AddPrimitive(primitive, materialProperties);
        }

        public new object Clone()
        {
            return new OurCollidableObject("clone - " + ID, //deep
                ActorType, //deep
                StatusType,
                Transform3D.Clone() as Transform3D, //deep
                EffectParameters.Clone() as OurEffectParameters, //hybrid - shallow (texture and effect) and deep (all other fields)
                Model,isBlocking); //shallow i.e. a reference
        }

        public virtual void Enable(bool bImmovable, float mass)
        {
            this.Mass = mass;

            //set whether the object can move
            Body.Immovable = bImmovable;
            //calculate the centre of mass
            Vector3 com = SetMass(mass);
            //adjust skin so that it corresponds to the 3D mesh as drawn on screen
            Body.MoveTo(Transform3D.Translation, Matrix.Identity);
            //set the centre of mass
            Collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            //enable so that any applied forces (e.g. gravity) will affect the object
            Body.EnableBody();
        }

        protected Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(PrimitiveProperties.MassDistributionEnum.Solid, PrimitiveProperties.MassTypeEnum.Density, mass);
            Collision.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);
            Body.BodyInertia = itCoM;
            Body.Mass = junk;

            return com;
        }

        #endregion
    }
}