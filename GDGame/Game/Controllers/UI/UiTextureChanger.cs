using System.Collections.Generic;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Controllers
{
    public class UiTextureChanger : Controller
    {
        #region Private variables

        private float cooldown;
        private float currentCd;
        private int i;
        private List<Texture2D> texture2Ds;
        private UITextureObject uiTextureObject;

        #endregion

        #region Constructors

        public UiTextureChanger(string id, ControllerType controllerType, List<Texture2D> texture2Ds,
            float cooldown) : base(id,
            controllerType)
        {
            this.texture2Ds = texture2Ds;
            this.cooldown = cooldown;
        }

        #endregion

        #region Override Method

        public override void Update(GameTime gameTime, IActor actor)
        {
            uiTextureObject ??= actor as UITextureObject;

            if (currentCd < cooldown)
            {
                currentCd += gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                if (uiTextureObject != null) uiTextureObject.Texture = texture2Ds[++i % texture2Ds.Count];

                currentCd = 0;
            }
        }

        #endregion
    }
}