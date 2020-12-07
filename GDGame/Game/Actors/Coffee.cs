using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Enums;
using GDGame.Game.Parameters.Effect;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Actors
{
    public class Coffee : PathMoveTile
    {
        public Coffee(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, List<CoffeeInfo> coffeeInfo) : base(id, actorType,
            statusType, transform, effectParameters, model, false, ETileType.None)
        {
        }

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
        
        
        
    }
}