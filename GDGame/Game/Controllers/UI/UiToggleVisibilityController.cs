using System.Runtime.Serialization;
using GDGame.Actors;
using GDGame.Managers;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame.Controllers
{
    public class UiToggleVisibilityController : Controller
    {
        private PlayerTile playerTile;
        private UITextureObject pressSpace;
        private OurObjectManager objectManager;

        public UiToggleVisibilityController(string id, ControllerType controllerType, OurObjectManager objectManager) :
            base(id, controllerType)
        {
            this.objectManager = objectManager;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            pressSpace ??= actor as UITextureObject;
            playerTile ??= objectManager.ActorList.Find(actor3D => actor3D.ActorType == ActorType.Player) as PlayerTile;

            if (pressSpace != null && playerTile != null)
            {
                if (playerTile != null && playerTile.AttachCandidates.Count > 0 && !playerTile.IsAttached)
                    pressSpace.StatusType = StatusType.Drawn | StatusType.Update;
                else
                    pressSpace.StatusType = StatusType.Update;
            }
        }
    }
}