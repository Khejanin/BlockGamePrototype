using System;
using GDGame.EventSystem;
using GDLibrary.Enums;
using GDLibrary.Managers;

//Physics - Step 2
namespace GDGame.Managers
{
    /// <summary>
    ///     Subclass of PhysicsManager that removes bodies on events.
    /// </summary>
    public class OurPhysicsManager : PhysicsManager, IDisposable
    {
        #region Constructors

        public OurPhysicsManager(Microsoft.Xna.Framework.Game game, StatusType statusType) : base(game, statusType)
        {
            EventManager.RegisterListener<RemoveActorEvent>(HandleRemoveActor);
        }

        #endregion

        #region Public Method

        public new void Dispose()
        {
            while (PhysicsSystem.Bodies.Count != 0) PhysicsSystem.RemoveBody(PhysicsSystem.Bodies[0]);
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