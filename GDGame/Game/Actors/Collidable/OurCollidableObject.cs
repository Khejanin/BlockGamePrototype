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
    ///     We have our own version due to OurEffectParameters again.
    /// </summary>
    public class OurCollidableObject : OurModelObject
    {
        #region Private variables

        private Vector3 com;
        private Matrix itCoM;

        private float junk;

        #endregion

        #region Constructors

        protected OurCollidableObject(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, bool isBlocking)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            IsBlocking = isBlocking;
            Body = new Body {ExternalData = this};
            Collision = new CollisionSkin(Body);
            Body.CollisionSkin = Collision;
        }

        #endregion

        #region Properties, Indexers

        public Body Body { get; }

        public CollisionSkin Collision { get; }

        public bool IsBlocking { get; }

        #endregion

        #region Override Method

        public override Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(Transform3D.Scale) * Collision.GetPrimitiveLocal(0).Transform.Orientation *
                   Body.Orientation * Transform3D.Orientation * Matrix.CreateTranslation(Body.Position);
        }

        #endregion

        #region Public Method

        public void AddPrimitive(Primitive primitive, MaterialProperties materialProperties)
        {
            Collision.AddPrimitive(primitive, materialProperties);
        }

        public new object Clone()
        {
            return new OurCollidableObject("clone - " + ID, ActorType, StatusType, Transform3D.Clone() as Transform3D,
                EffectParameters.Clone() as OurEffectParameters, Model, IsBlocking);
        }

        public virtual void Enable(bool bImmovable, float mass)
        {
            //set whether the object can move
            Body.Immovable = bImmovable;
            //calculate the centre of mass
            Vector3 centerOfMass = SetMass(mass);
            //adjust skin so that it corresponds to the 3D mesh as drawn on screen
            Body.MoveTo(Transform3D.Translation, Matrix.Identity);
            //set the centre of mass
            Collision.ApplyLocalTransform(new Transform(-centerOfMass, Matrix.Identity));
            //enable so that any applied forces (e.g. gravity) will affect the object
            Body.EnableBody();
        }

        #endregion

        #region Private Method

        private Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(
                PrimitiveProperties.MassDistributionEnum.Solid, PrimitiveProperties.MassTypeEnum.Density, mass);
            Collision.GetMassProperties(primitiveProperties, out junk, out com, out Matrix _, out itCoM);
            Body.BodyInertia = itCoM;
            Body.Mass = junk;

            return com;
        }

        #endregion
    }
}