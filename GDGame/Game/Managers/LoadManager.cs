using GDGame.Enums;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Managers
{
    /// <summary>
    ///     Class that Loads all the content we need for our game to work.
    /// </summary>
    public class LoadManager
    {
        #region Private variables

        private Main main;

        #endregion

        #region Constructors

        public LoadManager(Main main)
        {
            this.main = main;
        }

        #endregion

        #region Initialization

        public void InitLoad()
        {
            LoadEffects();
            LoadFonts();
            LoadModels();
            LoadSounds();
            LoadTextures();
            LoadBasicTextures();
        }

        #endregion

        #region Load Methods

        private void LoadBasicTextures()
        {
            main.Textures.Load("Assets/Textures/Menu/buttonWhite", "bStart");
        }

        private void LoadEffects()
        {
            main.Effects.Load("Assets/Effects/Normal");
            main.Effects.Load("Assets/Effects/Coffee");
        }

        private void LoadFonts()
        {
            main.Fonts.Load("Assets/Fonts/Arial");
        }

        private void LoadModels()
        {
            main.Models.Load("Assets/Models/box2", "Cube");
            main.Models.Load("Assets/Models/Button");
            main.Models.Load("Assets/Models/DropWithEyes", "Drop");
            main.Models.Load("Assets/Models/Fork");
            main.Models.Load("Assets/Models/Knife");
            main.Models.Load("Assets/Models/Mug");
            main.Models.Load("Assets/Models/PlateStack");
            main.Models.Load("Assets/Models/Player");
            main.Models.Load("Assets/Models/Pyramid");
            main.Models.Load("Assets/Models/SinglePlate");
            main.Models.Load("Assets/Models/SugarBox");
            main.Models.Load("Assets/Models/Decor/table01", "Table");
            main.Models.Load("Assets/Models/Decor/cups01", "Cups");
            main.Models.Load("Assets/Models/Decor/choco01", "Chocolate");
            main.Models.Load("Assets/Models/Decor/cat01", "Cat");
            main.Models.Load("Assets/Models/Decor/bed01", "CatBed");
            main.Models.Load("Assets/Models/Decor/coffeePot03", "Pot");
            main.Models.Load("Assets/Models/Decor/coffeeSpill", "Spill");
            main.Models.Load("Assets/Models/plane", "CoffeePlane");
            main.Models.Load("Assets/Models/Puddle");
            main.Models.Load("Assets/Models/Biscuit");
            main.Models.Load("Assets/Models/Smarties");
        }

        private void LoadSounds()
        {
            main.SoundManager.AddMusic("endTheme", main.Content.Load<SoundEffect>("Assets/GameTracks/track01"));
            main.SoundManager.AddMusic("gameTrack01", main.Content.Load<SoundEffect>("Assets/GameTracks/track02"));
            main.SoundManager.AddMusic("gameTrack02", main.Content.Load<SoundEffect>("Assets/GameTracks/track03"));
            main.SoundManager.AddMusic("gameTrack03", main.Content.Load<SoundEffect>("Assets/GameTracks/track04"));
            main.SoundManager.AddMusic("titleTheme", main.Content.Load<SoundEffect>("Assets/GameTracks/track05"));

            main.SoundManager.AddSoundEffect(SfxType.PlayerMove,
                main.Content.Load<SoundEffect>("Assets/Sound/Effects/step1"));
            main.SoundManager.AddSoundEffect(SfxType.PlayerAttach,
                main.Content.Load<SoundEffect>("Assets/Sound/Effects/attachBlock"));
            main.SoundManager.AddSoundEffect(SfxType.PlayerDetach,
                main.Content.Load<SoundEffect>("Assets/Sound/Effects/detachBlock"));
            main.SoundManager.AddSoundEffect(SfxType.CollectibleCollected,
                main.Content.Load<SoundEffect>("Assets/Sound/Effects/Collectible"));
            //try this one maybe
            //main.SoundManager.AddSoundEffect(SfxType.PlayerAttach, main.Content.Load<SoundEffect>("Assets/Sound/Effects/attach"));$

            main.SoundManager.AddSoundEffect(SfxType.EnemyMove,
                main.Content.Load<SoundEffect>("Assets/Sound/EnemySounds/old/move1"));
            main.SoundManager.AddSoundEffect(SfxType.TrapDeathWater,
                main.Content.Load<SoundEffect>("Assets/Sound/Effects/trapDeath"));
            main.SoundManager.AddSoundEffect(SfxType.MenuButtonClick,
                main.Content.Load<SoundEffect>("Assets/Sound/Effects/buttonClick"));

            //Taken from Sonniss.com, the GDC 2018 Free to use Sound archive, this one is part of UberDuo - The Home Barista
            main.SoundManager.AddSoundEffect(SfxType.CoffeeStart,
                main.Content.Load<SoundEffect>("Assets/Sound/Effects/CoffeePour"));

            main.SoundManager.StartMusicQueue();
        }

        private void LoadTextures()
        {
            main.Textures.Load("Assets/Textures/Props/GameTextures/TextureCube", "Finish");
            main.Textures.Load("Assets/Textures/Base/WhiteSquare");
            main.Textures.Load("Assets/Textures/Menu/menubaseres", "options");
            main.Textures.Load("Assets/Textures/Menu/button", "optionsButton");

            //Skybox
            main.Textures.Load("Assets/Textures/Skybox/kWall1");
            main.Textures.Load("Assets/Textures/Skybox/kWall2");
            main.Textures.Load("Assets/Textures/Skybox/kWall3");
            main.Textures.Load("Assets/Textures/Skybox/kWall4");
            main.Textures.Load("Assets/Textures/Skybox/tiles", "floor2");
            main.Textures.Load("Assets/Textures/Props/GameTextures/sugarbox");

            //Normals
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-normalmap");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-normalmap-choco", "big-normalmap_choco");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-normalmap-b_logic", "big-normalmap_b_logic");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-normalmap4x");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-normalmap8x");

            main.Textures.Load("Assets/Textures/Props/GameTextures/DisplacementMap", "big-displacement");

            //Chocolate, some of these would have better been tiled in the shader.
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco", "Chocolate");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco_choco", "Chocolate_choco");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco_b_logic", "Chocolate_b_logic");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-4x", "Chocolate4x");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-8x", "Chocolate8x");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-white", "WhiteChocolate");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-white_choco", "WhiteChocolate_choco");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-white_b_logic", "WhiteChocolate_b_logic");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-white4x", "WhiteChocolate4x");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-white8x", "WhiteChocolate8x");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-dark", "DarkChocolate");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-dark_choco", "DarkChocolate_choco");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-dark_b_logic", "DarkChocolate_b_logic");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-dark4x", "DarkChocolate4x");
            main.Textures.Load("Assets/Textures/Props/GameTextures/big-choco-dark8x", "DarkChocolate8x");

            //Coffee
            main.Textures.Load("Assets/Textures/uvalex", "CoffeeUV");
            main.Textures.Load("Assets/Textures/uvCoffeeDrop", "DropUV");
            main.Textures.Load("Assets/Textures/flowmap2", "CoffeeFlow");

            main.Textures.Load("Assets/Textures/Props/GameTextures/ceramicColoring", "Ceramic");
            main.Textures.Load("Assets/Textures/UI/CoffeeRisingWarning");
            main.Textures.Load("Assets/Textures/UI/CoffeeDangerWarning");

            //Sugar
            main.Textures.Load("Assets/Textures/Props/GameTextures/sugar01", "SugarW");
            main.Textures.Load("Assets/Textures/Props/GameTextures/sugar02", "SugarB");

            //Misc. Items
            main.Textures.Load("Assets/Textures/Props/GameTextures/MugTexture", "Mug");
            main.Textures.Load("Assets/Textures/Props/GameTextures/KnifeTexture", "Knife");
            main.Textures.Load("Assets/Textures/Props/GameTextures/ButtonTexture", "Button");

            main.Textures.Load("Assets/Textures/Props/GameTextures/wood", "Wood");
            main.Textures.Load("Assets/Textures/Props/GameTextures/blackTile");
            main.Textures.Load("Assets/Textures/Props/GameTextures/checkers", "Checkers");
            main.Textures.Load("Assets/Textures/Props/GameTextures/coffeeStrip", "coffeeSpill");

            //UI
            main.Textures.Load("Assets/Textures/Menu/menubaseres", "Menu");
            main.Textures.Load("Assets/Textures/Menu/Options");
            main.Textures.Load("Assets/Textures/Menu/EndScreen");
            main.Textures.Load("Assets/Textures/UI/Sad");
            main.Textures.Load("Assets/Textures/UI/Happy");
            main.Textures.Load("Assets/Textures/UI/RedSticker");
            main.Textures.Load("Assets/Textures/UI/GreenSticker");
            main.Textures.Load("Assets/Textures/UI/YellowSticker");
            main.Textures.Load("Assets/Textures/Props/GameTextures/Biscuit");
            main.Textures.Load("Assets/Textures/UI/TopBar");
            main.Textures.Load("Assets/Textures/UI/Mug-Collected");
            main.Textures.Load("Assets/Textures/Menu/tutorial", "Tutorial");
            main.Textures.Load("Assets/Textures/UI/PressSpace");
            main.Textures.Load("Assets/Textures/UI/PressSpace02");
            main.Textures.Load("Assets/Textures/UI/PressSpace03");
        }

        #endregion
    }
}