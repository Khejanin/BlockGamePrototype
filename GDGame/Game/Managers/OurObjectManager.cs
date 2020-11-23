using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Managers
{
    public class OurObjectManager : PausableDrawableGameComponent
    {
        #region Fields

        private CameraManager<Camera3D> cameraManager;
        private static List<DrawnActor3D> opaqueList;
        private static List<DrawnActor3D> transparentList;
        
        public ModelObject coffee;
        public RenderTarget2D withCoffee, withoutCoffee;
        public Effect coffeePostProcess;
        public Vector2 screenSpace;
        public Texture2D testTexture;
        public SpriteBatch spriteBatch;
        
        #endregion Fields

        #region Constructors & Core

        public OurObjectManager(Microsoft.Xna.Framework.Game game, StatusType statusType,
          int initialOpaqueDrawSize, int initialTransparentDrawSize,
          CameraManager<Camera3D> cameraManager) : base(game, statusType)
        {
            this.cameraManager = cameraManager;
            opaqueList = new List<DrawnActor3D>(initialOpaqueDrawSize);
            transparentList = new List<DrawnActor3D>(initialTransparentDrawSize);

            //        EventDispatcherV2.Subscribe(EventCategoryType.Menu, HandleMenuChanged);

            SubscribeToEvents();
        }

        protected override void SubscribeToEvents()
        {
            //menu
            EventDispatcher.Subscribe(EventCategoryType.Menu, HandleEvent);


            //remove
            EventDispatcher.Subscribe(EventCategoryType.Object, HandleEvent);

            //add

            //transparency
        }

        protected override void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Menu)
            {
                if (eventData.EventActionType == EventActionType.OnPause)
                    this.StatusType = StatusType.Off;
                else if (eventData.EventActionType == EventActionType.OnPlay)
                    this.StatusType = StatusType.Drawn | StatusType.Update;
            }
            else if (eventData.EventCategoryType == EventCategoryType.Object)
            {
                if (eventData.EventActionType == EventActionType.OnRemoveActor)
                {
                    DrawnActor3D removeObject = eventData.Parameters[0] as DrawnActor3D;

                    opaqueList.Remove(removeObject);

                }
            }
        }

        public void Add(DrawnActor3D actor)
        {
            if (actor.EffectParameters.Alpha < 1)
                transparentList.Add(actor);
            else
                opaqueList.Add(actor);
        }

        public bool RemoveFirstIf(Predicate<DrawnActor3D> predicate)
        {
            //to do....
            int position = opaqueList.FindIndex(predicate);

            if (position != -1)
            {
                opaqueList.RemoveAt(position);
                return true;
            }

            return false;
        }

        public int RemoveAll(Predicate<DrawnActor3D> predicate)
        {
            //to do....
            return opaqueList.RemoveAll(predicate);
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);
            }
        }

        protected override void ApplyDraw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(withCoffee);
            
            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, cameraManager.ActiveCamera, GraphicsDevice);
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, cameraManager.ActiveCamera, GraphicsDevice);
            }
            
            //GraphicsDevice.BlendState = BlendState.AlphaBlend;
            
            coffeePostProcess.Parameters["time"].SetValue((float)(gameTime.TotalGameTime.TotalMilliseconds/100000));
            coffeePostProcess.Parameters["tex"].SetValue(testTexture);
            coffeePostProcess.CurrentTechnique.Passes[0].Apply();
            foreach (ModelMesh mesh in coffee.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = coffeePostProcess;
                }
                coffeePostProcess.Parameters["WorldViewProjection"].SetValue(coffee.BoneTransforms[mesh.ParentBone.Index] * coffee.Transform3D.World * cameraManager.ActiveCamera.View * cameraManager.ActiveCamera.Projection);
                mesh.Draw();
            }
            
            GraphicsDevice.BlendState = BlendState.Opaque;

            GraphicsDevice.SetRenderTarget(withoutCoffee);
            
            foreach (DrawnActor3D actor in opaqueList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, cameraManager.ActiveCamera, GraphicsDevice);
            }

            foreach (DrawnActor3D actor in transparentList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, cameraManager.ActiveCamera, GraphicsDevice);
            }

            GraphicsDevice.SetRenderTarget(null);
            
            //coffeePostProcess.Parameters["WithoutCoffee"].SetValue(withoutCoffee);
            //coffeePostProcess.Parameters["WithCoffee"].SetValue(withCoffee);
            //coffeePostProcess.Parameters["time"].SetValue(gameTime.TotalGameTime.Milliseconds);
            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, 
                SamplerState.LinearClamp, DepthStencilState.Default, 
                RasterizerState.CullNone);
            
            //coffeePostProcess.CurrentTechnique.Passes[0].Apply();
 
            spriteBatch.Draw(withCoffee, new Rectangle(0, 0,(int) screenSpace.X,(int) screenSpace.Y ),Color.White);
 
            spriteBatch.End();
            
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        #endregion Constructors & Core

        public List<DrawnActor3D> FindAll(Predicate<DrawnActor3D> predicate)
        {
            List<DrawnActor3D> result = opaqueList.FindAll(predicate);
            result.AddRange(transparentList.FindAll(predicate));
            return result;
        }
        
        public static List<DrawnActor3D> GetAllObjects()
        {
            List<DrawnActor3D> result = new List<DrawnActor3D>();
            result.AddRange(opaqueList);
            result.AddRange(transparentList);
            return result;
        }
    }
}