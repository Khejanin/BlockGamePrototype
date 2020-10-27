using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.GDLibrary.Actor
{
    public class DummyPlayer : ModelObject
    {

        enum Direction
        {
            Right,
            Left,
            Forward,
            Back
        }
        
        struct PlayerSurroundCheck
        {
            public Direction dir;
            public Raycaster.HitResult hit;
        }

        public void OnMoved()
        {
            
        }

        private List<PlayerSurroundCheck> CheckSurroundings()
        {
            List<PlayerSurroundCheck> result = new List<PlayerSurroundCheck>();
            
            PlayerSurroundCheck surroundCheck = new PlayerSurroundCheck();
            surroundCheck.hit = this.Raycast(Transform3D.Translation, Vector3.Right,true);
            surroundCheck.dir = Direction.Right;
            result.Add(surroundCheck);
            
            surroundCheck.hit = this.Raycast(Transform3D.Translation, -Vector3.Right,true);
            surroundCheck.dir = Direction.Left;
            result.Add(surroundCheck);
            
            surroundCheck.hit = this.Raycast(Transform3D.Translation, Vector3.Forward,true);
            surroundCheck.dir = Direction.Forward;
            result.Add(surroundCheck);
            
            surroundCheck.hit = this.Raycast(Transform3D.Translation, -Vector3.Forward,true);
            surroundCheck.dir = Direction.Back;
            result.Add(surroundCheck);

            return result;
        }

        public DummyPlayer(string id, ActorType actorType, StatusType statusType, Transform3D transform, EffectParameters effectParameters, Model model, RasterizerState rasterizerState = null) : base(id, actorType, statusType, transform, effectParameters, model, rasterizerState)
        {
        }
    }
}