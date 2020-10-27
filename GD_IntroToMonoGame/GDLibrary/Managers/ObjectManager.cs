using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using SharpDX.Direct3D11;
using RasterizerState = Microsoft.Xna.Framework.Graphics.RasterizerState;

namespace GDLibrary
{
    public class ObjectManager : DrawableGameComponent
    {
        private CameraManager cameraManager;
        private static List<DrawnActor3D> opaqueList, transparentList;

        public ObjectManager(Game game, 
            int initialOpaqueDrawSize, int initialTransparentDrawSize,
            CameraManager cameraManager) : base(game)
        {
            this.cameraManager = cameraManager;
            opaqueList = new List<DrawnActor3D>(initialOpaqueDrawSize);
            transparentList = new List<DrawnActor3D>(initialTransparentDrawSize);
        }

        public static List<DrawnActor3D> GetAllObjects()
        {
            List<DrawnActor3D> result = new List<DrawnActor3D>();
            result.AddRange(opaqueList);
            result.AddRange(transparentList);
            return result;
        }

        public void Add(DrawnActor3D actor)
        {
            if (actor.EffectParameters.Alpha < 1)
                transparentList.Add(actor);
            else
                opaqueList.Add(actor);
        }

        public List<DrawnActor3D> FindAll(Predicate<DrawnActor3D> predicate)
        {
            List<DrawnActor3D> result = opaqueList.FindAll(predicate);
            result.AddRange(transparentList.FindAll(predicate));
            return result;
        }
        
        public bool RemoveIf(Predicate<DrawnActor3D> predicate)
        {

         //   RemoveIf(actor => actor.ID.Equals("dungeon powerup key"));

            int position = opaqueList.FindIndex(predicate);

            if (position != -1)
            {
                opaqueList.RemoveAt(position);
                return true;
            }

            return false;
        }

        public int RemoveAllIf(Predicate<DrawnActor3D> predicate)
        {
            //to do...
            return -1;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (DrawnActor3D actor in opaqueList)
            {
                if((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            RasterizerState defaultRasterizerState = GraphicsDevice.RasterizerState;
            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, 
                       this.cameraManager.ActiveCamera,
                        this.GraphicsDevice);
                
                if (GraphicsDevice.RasterizerState != defaultRasterizerState)
                    GraphicsDevice.RasterizerState = defaultRasterizerState;
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime,
                       this.cameraManager.ActiveCamera,
                        this.GraphicsDevice);
                
                if (GraphicsDevice.RasterizerState != defaultRasterizerState)
                    GraphicsDevice.RasterizerState = defaultRasterizerState;
            }
        }




    }
}
