using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Controllers
{
    /// <summary>
    /// Parent class for all controllers in the game which adds the ControllerType and id fields
    /// </summary>
    public class Controller : IController
    {
        #region Fields

        private string id;
        private ControllerType controllerType;

        #endregion Fields

        #region Properties

        protected string Id { get => id; set => id = value.Trim(); }
        protected ControllerType ControllerType { get => controllerType; set => controllerType = value; }

        public ControllerType GetControllerType()
        {
            return ControllerType;
        }

        #endregion Properties

        protected Controller(string id, ControllerType controllerType)
        {
            Id = id;
            ControllerType = controllerType;
        }

        public virtual void Update(GameTime gameTime, IActor actor)
        {
            //does nothing - see child classes
        }

        public virtual object Clone()
        {
            return new Controller(id, controllerType);
        }
    }
}