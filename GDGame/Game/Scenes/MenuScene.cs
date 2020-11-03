using GDGame.Scenes;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.Game.Scenes
{
    public class MenuScene : Scene
    {
        public MenuScene(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            InitializeCamera();
            InitializeTextures();
        }

        protected void InitializeCamera()
        {
            Camera3D camera3D = new Camera3D("Menu Camera",ActorType.Camera3D,StatusType.Update,new Transform3D(Vector3.Zero, -Vector3.Forward,Vector3.Up),game.GlobalProjectionParameters);
            CameraManager.Add(camera3D);
        }

        protected void InitializeTextures()
        {
            
        }
        
        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            
        }

        public override void Terminate()
        {
            
        }
    }
}