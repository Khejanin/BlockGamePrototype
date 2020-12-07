using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Game.Parameters.Effect;
using GDGame.Managers;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plane = JigLibX.Geometry.Plane;

namespace GDGame.Game.Actors
{
    public class Coffee : PathMoveTile
    {
        private int setBackIndex = 0;
        
        private List<CoffeeInfo> coffeeInfo;
        
        public List<CoffeeInfo> CoffeeInfo
        {
            get
            {
                return coffeeInfo;
            }
            set
            {
                Path.Clear();
                
                foreach (CoffeeInfo info in value)
                {
                    Path.Add(new Vector3(0,info.Y,0));
                }

                coffeeInfo = value;
            }
        }
        
        public Coffee(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, List<CoffeeInfo> coffeeInfo) : base(id, actorType,
            statusType, transform, effectParameters, model, false, ETileType.None)
        {
            CoffeeInfo = coffeeInfo;
            InitializeCollision(-Vector3.Up * coffeeInfo[0].Y);
            InitializeTile();
        }

        public override void SetTranslation(Vector3 translation)
        {
            base.SetTranslation(translation - Vector3.Up);
            Transform3D.Translation = translation;
        }

        public override void InitializeCollision(Vector3 position, float factor = 1)
        {
            SetTranslation(position);
            AddPrimitive(new Box(Transform3D.Translation - Vector3.Up, Matrix.Identity,new Vector3(100,0.01f,100)),
                new MaterialProperties(1,1,1));
            Enable(true, 1);
        }


        public override void InitializeTile()
        {
            EventManager.RegisterListener<PlayerEventInfo>(CheckPointReached);
        }

        private void CheckPointReached(PlayerEventInfo info)
        {
            if (info.type == PlayerEventType.SetCheckpoint)
            {
                this.MoveTo(new AnimationEventData()
                {
                    isRelative = true,
                    body = Body,
                    destination = -Vector3.Up * coffeeInfo[setBackIndex++].SetBackY,
                    smoothing = Smoother.SmoothingMethod.Decelerate,
                    maxTime = 500
                });
            }
        }
        
    }
}