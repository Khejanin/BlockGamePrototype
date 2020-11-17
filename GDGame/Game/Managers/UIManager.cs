using System.Collections.Generic;
using System.Linq;
using GDGame.Game.UI;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Managers
{
    public class UiManager : DrawableGameComponent
    {
        private Dictionary<string, UiElement> elements;
        private SpriteBatch spriteBatch;
        

        public UiManager(Microsoft.Xna.Framework.Game game) : base(game)
        {
            elements = new Dictionary<string, UiElement>();
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public int Count => elements.Count;

        public void AddUiElement(string name, UiElement element)
        {
            elements.Add(name, element);
        }

        //in game menu overlay trigger
        public void Options(bool q)
        {
            foreach (KeyValuePair<string, UiElement> keyValuePair in elements)
            {
                if(keyValuePair.Key.Contains("Options"))
                {
                    if(q)
                    {
                        keyValuePair.Value.StatusType = StatusType.Drawn;
                    }
                    else { keyValuePair.Value.StatusType = StatusType.Off; }
                    
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (KeyValuePair<string, UiElement> keyValuePair in elements.Where(keyValuePair =>
                keyValuePair.Value.StatusType == StatusType.Drawn))
            {
                keyValuePair.Value.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            base.Draw(gameTime);
        }

        public UiElement this[string key] => elements[key];

        public void Clear()
        {
            elements.Clear();
        }
    }
}