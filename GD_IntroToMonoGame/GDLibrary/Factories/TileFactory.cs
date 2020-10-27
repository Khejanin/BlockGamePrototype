﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GDLibrary
{
    public class TileFactory
    {
        ObjectManager objectManager;
        KeyboardManager keyboardManager;
        ContentManager contentManager;
        BasicEffect modelEffect;

        public TileFactory(KeyboardManager keyboardManager, ObjectManager objectManager, ContentManager contentManager, BasicEffect modelEffect)
        {
            this.keyboardManager = keyboardManager;
            this.objectManager = objectManager;
            this.contentManager = contentManager;
            this.modelEffect = modelEffect;
        }

        public GridTile CreateTile(ETileType type)
        {
            GridTile tile = null;

            switch (type)
            {
                case ETileType.PlayerStart:
                    tile = CreatePlayer(new PlayerController(keyboardManager));
                    break;
                case ETileType.Static:
                    tile = CreateStatic();
                    break;
                case ETileType.Attachable:
                    tile = CreateAttachable();
                    break;
            }

            return tile;
        }

        private GridTile CreateStatic()
        {
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                contentManager.Load<Texture2D>("Assets/Textures/Props/Crates/crate1"),
                Color.White, 1);
            Model model = contentManager.Load<Model>("Assets/Models/box2");
            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            StaticTile staticTile = new StaticTile("StaticTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, model);
            objectManager.Add(staticTile);
            return staticTile;
        }

        private GridTile CreateAttachable()
        {
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                contentManager.Load<Texture2D>("Assets/Textures/Props/Crates/crate1"),
                Color.White, 1);
            Model model = contentManager.Load<Model>("Assets/Models/box2");
            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GridTile attachableTile = new GridTile("AttachableTile", ActorType.Primitive, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, model);
            objectManager.Add(attachableTile);
            return attachableTile;
        }

        private GridTile CreatePlayer(PlayerController controller)
        {
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                contentManager.Load<Texture2D>("Assets/Textures/Props/Crates/crate1"),
                Color.White, 1);
            Model model = contentManager.Load<Model>("Assets/Models/box2");
            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            CubePlayer player = new CubePlayer("Player1", ActorType.Player, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, model);
            player.ControllerList.Add(controller);
            objectManager.Add(player);
            return player;
        }
    }
}