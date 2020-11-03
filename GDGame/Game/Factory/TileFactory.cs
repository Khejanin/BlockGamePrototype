using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GDGame.Game.Tiles;
using GDLibrary;
using GDLibrary.Managers;
using GDGame.Game.Enums;
using GDLibrary.Enums;
using GDLibrary.Parameters;

namespace GDGame.Game.Factory
{
    public class TileFactory
    {
        private ObjectManager objectManager;
        private KeyboardManager keyboardManager;
        private BasicEffect modelEffect;
        private SpriteFont font;

        private List<Model> models;
        private List<Texture2D> textures;

        public TileFactory(KeyboardManager keyboardManager, ObjectManager objectManager,
            BasicEffect modelEffect, SpriteFont font, List<Model> models, List<Texture2D> textures)
        {
            this.keyboardManager = keyboardManager;
            this.objectManager = objectManager;
            this.modelEffect = modelEffect;
            this.font = font;
            this.models = models;
            this.textures = textures;
        }

        public GridTile CreateTile(ETileType type)
        {
            GridTile tile = type switch
            {
                ETileType.PlayerStart => CreatePlayer(new PlayerController(keyboardManager)),
                ETileType.Static => CreateStatic(),
                ETileType.Attachable => CreateAttachable(),
                _ => null
            };

            if (tile != null) tile.TileType = type;
            return tile;
        }

        public Shape CreateShape()
        {
            return new Shape("Shape", ActorType.NonPlayer, StatusType.Update,
                new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY));
        }

        private GridTile CreateStatic()
        {
            EffectParameters effectParameters = new EffectParameters(modelEffect, textures[0], Color.White, 1);
            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            StaticTile staticTile = new StaticTile("StaticTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models[0]);
            objectManager.Add(staticTile);
            return staticTile;
        }

        private GridTile CreateAttachable()
        {
            EffectParameters effectParameters = new EffectParameters(modelEffect, textures[1], Color.White, 1);
            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            AttachableTile attachableTile = new AttachableTile("AttachableTile", ActorType.Primitive,
                StatusType.Drawn | StatusType.Update, transform3D, effectParameters, models[1]);
            objectManager.Add(attachableTile);
            return attachableTile;
        }

        private GridTile CreatePlayer(PlayerController controller)
        {
            EffectParameters effectParameters = new EffectParameters(modelEffect, textures[1], Color.White, 1);
            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            CubePlayer player = new CubePlayer("Player1", ActorType.Player, StatusType.Drawn | StatusType.Update,
                transform3D, effectParameters, models[2], font);
            player.ControllerList.Add(controller);
            objectManager.Add(player);
            return player;
        }
    }
}