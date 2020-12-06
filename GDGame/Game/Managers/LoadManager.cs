using GDGame.Enums;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Managers
{
    public class LoadManager
    {
        private Main main;

        public LoadManager(Main main)
        {
            this.main = main;
        }

        public void InitLoad()
        {
            LoadEffects();
            LoadFonts();
            LoadModels();
            LoadSounds();
            LoadTextures();
            LoadBasicTextures();
        }
        
        #region Load Methods

        private void LoadBasicTextures()
        {
            main.Textures.Load("Assets/Textures/Menu/button", "bStart");
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
        }

        private void LoadSounds()
        {
            SoundEffect track01 = main.Content.Load<SoundEffect>("Assets/GameTracks/track01");
            SoundEffect track02 = main.Content.Load<SoundEffect>("Assets/GameTracks/track02");
            SoundEffect track03 = main.Content.Load<SoundEffect>("Assets/GameTracks/track03");
            SoundEffect track04 = main.Content.Load<SoundEffect>("Assets/Sound/Knock04");
            SoundEffect track05 = main.Content.Load<SoundEffect>("Assets/Sound/Click02");
            SoundEffect track06 = main.Content.Load<SoundEffect>("Assets/GameTracks/track04");
            SoundEffect track07 = main.Content.Load<SoundEffect>("Assets/GameTracks/track05");

            main.SoundManager.AddMusic("endTheme", track01);
            main.SoundManager.AddMusic("gameTrack01", track02);
            main.SoundManager.AddMusic("gameTrack02", track03);
            main.SoundManager.AddMusic("gameTrack03", track06);
            main.SoundManager.AddMusic("titleTheme", track07);
            main.SoundManager.AddSoundEffect(SfxType.PlayerMove, track04);
            main.SoundManager.AddSoundEffect(SfxType.PlayerAttach, track05);

            main.SoundManager.StartMusicQueue();
        }

        private void LoadTextures()
        {
            main.Textures.Load("Assets/Textures/Props/GameTextures/TextureCube", "Finish");

            main.Textures.Load("Assets/Textures/Base/WhiteSquare");
            main.Textures.Load("Assets/Textures/UI/TopBar");
            main.Textures.Load("Assets/Textures/UI/Mug-Collected");


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

            //Chocolate
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

            main.Textures.Load("Assets/Textures/Props/GameTextures/sugar01", "SugarW");
            main.Textures.Load("Assets/Textures/Props/GameTextures/sugar02", "SugarB");

            main.Textures.Load("Assets/Textures/Props/GameTextures/MugTexture", "Mug");
            main.Textures.Load("Assets/Textures/Props/GameTextures/KnifeTexture", "Knife");
            main.Textures.Load("Assets/Textures/Props/GameTextures/ButtonTexture", "Button");

            main.Textures.Load("Assets/Textures/Props/GameTextures/wood", "Wood");
            main.Textures.Load("Assets/Textures/Props/GameTextures/blackTile");
            main.Textures.Load("Assets/Textures/Props/GameTextures/checkers", "Checkers");
            main.Textures.Load("Assets/Textures/Props/GameTextures/coffeeStrip", "coffeeSpill");

            main.Textures.Load("Assets/Textures/Menu/menubaseres", "Menu");
            main.Textures.Load("Assets/Textures/Menu/Options");
        }

        #endregion
    }
}