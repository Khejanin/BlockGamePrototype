using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace RefractionShader
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Effect refractionEffect;

        Texture2D tex2dForground;
        Texture2D tex2dRefractionTexture;

        Texture2D tex2dXnaUkImage;
        Texture2D tex2dcool02;
        Texture2D tex2dEgyptianDesert;
        Texture2D tex2dfire01;
        Texture2D tex2dbackground_mat_rgo;
        Texture2D tex2dpgfkp;
        Texture2D tex2dwater04;
        Texture2D tex2dRefractionImg;

        // some default control values for the refractions.
        float speed = .02f;
        float waveLength = .08f;
        float frequency = .2f;
        float refractiveIndex = 1.3f;
        Vector2 refractionVector = new Vector2(0f, 0f);
        float refractionVectorRange = 100;

        // To alter the size and position on the screen of all the image squares.
        // Change the w h values.
        public int StW{get;set;}
        public int StH{get;set;}


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 800;
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            StW = graphics.PreferredBackBufferWidth / 4;
            StH = graphics.PreferredBackBufferHeight /3;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
//            font = Content.Load<SpriteFont>("MgGenFont");

            refractionEffect = Content.Load<Effect>("Assets/Effects/refrac");

            // extra images for testing.
            tex2dXnaUkImage = Content.Load<Texture2D>("Assets/Textures/xnarefracdemo");
            //tex2dcool02 = Content.Load<Texture2D>("cool02");
            //tex2dEgyptianDesert = Content.Load<Texture2D>("EgyptianDesert");
            //tex2dfire01 = Content.Load<Texture2D>("fire01");
            //tex2dpgfkp = Content.Load<Texture2D>("pgfkp");
            tex2dRefractionImg = Content.Load<Texture2D>("Assets/Textures/xnarefracnormalpng");
            //tex2dwater04 = Content.Load<Texture2D>("water04");
            //tex2dbackground_mat_rgo = Content.Load<Texture2D>("background_mat_rgo");

            // the primary two textures used for all the tests.
            tex2dForground = tex2dXnaUkImage;
            tex2dRefractionTexture = tex2dRefractionImg;
        }

        protected override void UnloadContent()
        {
        }

        float pauseTimer = 1.0f;
        float pauseDelay = .5f;
        int choice = 0;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (pauseTimer > 0f)
                pauseTimer = pauseTimer - (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    choice++;
                    if (choice > 3)
                        choice = 0;
                    switch (choice)
                    {
                        case 1:
                            tex2dForground = tex2dEgyptianDesert;
                            break;
                        case 2:
                            tex2dForground = tex2dwater04;
                            break;
                        case 3:
                            tex2dForground = tex2dfire01;
                            break;
                        default:
                            tex2dForground = tex2dXnaUkImage;
                            break;
                    }
                    pauseTimer = pauseDelay;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    waveLength += .001f;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    waveLength -= .001f;
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    frequency += .001f;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    frequency -= .001f;
            }

            
            refractionVector = Mouse.GetState().Position.ToVector2();
            refractionVector.Y = graphics.GraphicsDevice.Viewport.Bounds.Size.ToVector2().Y - refractionVector.Y;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {      
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the images well be using.

            // row 1  far left we squeeze the regular two images in here that we use.

            spriteBatch.Begin();
            spriteBatch.Draw(tex2dRefractionTexture, new Rectangle(StW * 0, StH * 0, StW, StH/2), Color.White);
            spriteBatch.Draw(tex2dForground, new Rectangle(StW * 0, StH /2, StW, StH/2), Color.White);
            //spriteBatch.DrawString(font, "wavelength " + waveLength + "\nfrequency " + frequency, new Vector2(10, 10), Color.Wheat);
            //spriteBatch.DrawString(font, "Mouse: " + refractionVector, new Vector2(10, 60), Color.Moccasin);
            spriteBatch.End();

            // row 2 and 3 far left
            Draw2dRefractionTechnique("RefractMonoCromeClipDarkness", tex2dForground, tex2dRefractionTexture, new Rectangle(StW * 0, StH * 1, StW, StH), speed, refractiveIndex, frequency, waveLength, refractionVector, refractionVectorRange, new Vector2(10, 1), false, gameTime);
            Draw2dRefractionTechnique("RefractDiagnalAverageMonochrome", tex2dForground, tex2dRefractionTexture, new Rectangle(StW * 0, StH * 2, StW, StH), speed, refractiveIndex, frequency, waveLength, refractionVector, refractionVectorRange, new Vector2(10, 1), false, gameTime);

            // row1

            Draw2dRefractionTechnique("Refraction2", tex2dForground, tex2dRefractionTexture, new Rectangle(StW*1, StH*0, StW, StH), speed, refractiveIndex, frequency, waveLength, refractionVector, refractionVectorRange, new Vector2(10, 1), false, gameTime);
            Draw2dRefractionTechnique("Refraction2", tex2dForground, tex2dRefractionTexture, new Rectangle(StW * 2, StH * 0, StW, StH), speed, refractiveIndex, frequency, waveLength, refractionVector, refractionVectorRange, new Vector2(10, 1), true, gameTime);       
            Draw2dRefractionTechnique("RefractionSnells", tex2dForground, tex2dRefractionTexture, new Rectangle(StW * 3, StH * 0, StW, StH), speed, refractiveIndex, frequency, waveLength, refractionVector, refractionVectorRange, new Vector2(10, 1), false, gameTime);

            // row 2

            Draw2dRefractionTechnique("RefractionMap", tex2dForground, tex2dRefractionTexture, new Rectangle(StW * 1, StH * 1, StW, StH), speed, refractiveIndex, frequency, waveLength, refractionVector, refractionVectorRange, new Vector2(10, 1), false, gameTime);
            Draw2dRefractionTechnique("RefractionMap", tex2dForground, tex2dRefractionTexture, new Rectangle(StW * 2, StH * 1, StW, StH), speed, refractiveIndex, frequency, waveLength, refractionVector, refractionVectorRange, new Vector2(10, 1), true, gameTime);
            Draw2dRefractionTechnique("TwoPassTechnique", tex2dForground, tex2dRefractionTexture, new Rectangle(StW * 3, StH * 1, StW, StH), speed, refractiveIndex, frequency, waveLength, refractionVector, refractionVectorRange, new Vector2(10, 1), false, gameTime);

            // row 3 

            Draw2dRefractionTechnique("RefractGoldenRatioHighlight", tex2dForground, tex2dRefractionTexture, new Rectangle(StW * 1, StH * 2, StW, StH), speed, refractiveIndex, frequency, waveLength, refractionVector, refractionVectorRange, new Vector2(10, 1), false, gameTime);
                Draw2dRefractionTechnique("RefractAntiRefractionArea", tex2dForground, tex2dRefractionTexture, new Rectangle(StW * 2, StH * 2, StW, StH), speed, refractiveIndex, frequency, waveLength, refractionVector, refractionVectorRange, new Vector2(10, 1), false, gameTime);
                Draw2dRefractionTechnique("LimitedRefractionArea", tex2dForground, tex2dRefractionTexture, new Rectangle(StW * 3, StH * 2, StW, StH), speed, refractiveIndex, frequency, waveLength, refractionVector, refractionVectorRange, new Vector2(10, 1), true, gameTime);

                // Draw to the whole screen for testing.
                //Draw2dRefractionTechnique("RefractAntiRefractionArea", tex2dForground, tex2dRefractionTexture, new Rectangle(0, 0, StW * 4, StH * 3), speed, refractiveIndex, frequency, waveLength, new Vector2(10, 1), true, gameTime);
                
                base.Draw(gameTime);
            }

            /// <summary>
            /// Draw a refracted texture using the refraction technique.
            /// </summary>
            public void Draw2dRefractionTechnique(string technique, Texture2D texture, Texture2D displacementTexture, Rectangle screenRectangle, float refractionSpeed, float refractiveIndex, float frequency, float sampleWavelength, Vector2 refractionVector, float refractionVectorRange, Vector2 windDirection, bool useWind, GameTime gameTime)
            {
                Vector2 displacement;
                double time = gameTime.TotalGameTime.TotalSeconds * refractionSpeed;
                if (useWind)
                    displacement = -(Vector2.Normalize(windDirection) * (float)time);
                else
                    displacement = new Vector2((float)Math.Cos(time), (float)Math.Sin(time));

                // Set an effect parameter to make the displacement texture scroll in a giant circle.
                refractionEffect.CurrentTechnique = refractionEffect.Techniques[technique];
                refractionEffect.Parameters["DisplacementTexture"].SetValue(displacementTexture);
                refractionEffect.Parameters["DisplacementMotionVector"].SetValue(displacement);
                refractionEffect.Parameters["SampleWavelength"].SetValue(sampleWavelength);
                refractionEffect.Parameters["Frequency"].SetValue(frequency);
                refractionEffect.Parameters["RefractiveIndex"].SetValue(refractiveIndex);
                // for the very last little test.
                refractionEffect.Parameters["RefractionVector"].SetValue(refractionVector);
                refractionEffect.Parameters["RefractionVectorRange"].SetValue(refractionVectorRange);
                

                spriteBatch.Begin(0, null, null, null, null, refractionEffect);
                spriteBatch.Draw(texture, screenRectangle, Color.White);
                spriteBatch.End();

                DisplayName(screenRectangle, technique, useWind);
            }

            public void DisplayName(Rectangle screenRectangle , string technique, bool useWind)
            {
                spriteBatch.Begin();
                var offset = screenRectangle;
                offset.Location += new Point(20, 20);
                //spriteBatch.DrawString(font, technique, offset.Location.ToVector2(), Color.White);
                if (useWind)
                {
                    offset.Location += new Point(0, 30);
                    //spriteBatch.DrawString(font, "wind on", offset.Location.ToVector2(), Color.White);
                }
                spriteBatch.End();
            }

        }
    }