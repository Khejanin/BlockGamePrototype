using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using JigLibX.Collision;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using GDGame.EventSystem;
using GDLibrary.Managers;

//Physics - Step 2
namespace GDGame.Managers
{
    /// <summary>
    /// Subclass of PhysicsManager that removes bodies on events. 
    /// </summary>
    public class OurPhysicsManager : PhysicsManager
    {
        public OurPhysicsManager(Microsoft.Xna.Framework.Game game, StatusType statusType) : base(game, statusType)
        {
            EventManager.RegisterListener<RemoveActorEvent>(HandleRemoveActor);
        }

        public OurPhysicsManager(Microsoft.Xna.Framework.Game game, StatusType statusType, Vector3 gravity) : base(game, statusType, gravity)
        {
            EventManager.RegisterListener<RemoveActorEvent>(HandleRemoveActor);
        }
        
        private void HandleRemoveActor(RemoveActorEvent obj)
        {
            if(obj.body != null) PhysicsSystem.RemoveBody(obj.body);
        }
        
    }
}