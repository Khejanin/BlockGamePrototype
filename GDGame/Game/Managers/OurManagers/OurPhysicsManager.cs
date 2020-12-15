using System;
using GDGame.EventSystem;
using GDLibrary.Enums;
using GDLibrary.Managers;
using JigLibX.Physics;

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

        #region Events

        private void HandleRemoveActor(RemoveActorEvent obj)
        {
            if (obj.body != null) PhysicsSystem.RemoveBody(obj.body);
        }

        #endregion

        public new void Dispose()
        {
            for (int i = 0; i < PhysicsSystem.Bodies.Count; i++)
            {
                PhysicsSystem.RemoveBody(PhysicsSystem.Bodies[i]);
            }
        }
    }
}