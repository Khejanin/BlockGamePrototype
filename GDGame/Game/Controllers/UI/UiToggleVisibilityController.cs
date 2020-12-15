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
        #region Private variables

        private OurObjectManager objectManager;
        private PlayerTile playerTile;
        private UITextureObject pressSpace;

        #endregion

        #region Constructors

        public UiToggleVisibilityController(string id, ControllerType controllerType, OurObjectManager objectManager) :
            base(id, controllerType)
        {
            this.objectManager = objectManager;
        }

        #endregion

        #region Override Method

        public override void Dispose()
        {
            objectManager = null;
            playerTile = null;
            pressSpace = null;
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

        #endregion
    }
}