using GDGame.EventSystem;
using GDLibrary.Enums;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;

//Physics - Step 2
namespace GDGame.Managers
{
    /// <summary>
    ///     Subclass of PhysicsManager that removes bodies on events.
    /// </summary>
    public class OurPhysicsManager : PhysicsManager
    {
        #region Constructors

        public OurPhysicsManager(Microsoft.Xna.Framework.Game game, StatusType statusType) : base(game, statusType)
        {
            EventManager.RegisterListener<RemoveActorEvent>(HandleRemoveActor);
        }

        public OurPhysicsManager(Microsoft.Xna.Framework.Game game, StatusType statusType, Vector3 gravity) : base(game,
            statusType, gravity)
        {
            EventManager.RegisterListener<RemoveActorEvent>(HandleRemoveActor);
        }

        #endregion

        #region Events

        private void HandleRemoveActor(RemoveActorEvent obj)
        {
            if (obj.body != null) PhysicsSystem.RemoveBody(obj.body);
        }

        #endregion
    }
}