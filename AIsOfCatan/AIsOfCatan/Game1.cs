using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TroyXnaAPI;

namespace AIsOfCatan
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : TXAGame
    {
        //GraphicsDeviceManager graphics;
        //SpriteBatch spriteBatch;
        readonly GameState state = new GameState(new Board(1),null,null,null,0);

        public const float SCALE = 0.5f;

        private TXAScreen startScreen;

        public Game1()
        {
            //graphics = new GraphicsDeviceManager(this);
            //Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ARIAL = Content.Load<SpriteFont>("Arial");
            TEXTURES.Add("T_Desert", Content.Load<Texture2D>("DesertTile"));
            TEXTURES.Add("T_Fields", Content.Load<Texture2D>("FieldsTile"));
            TEXTURES.Add("T_Forest", Content.Load<Texture2D>("ForestTile"));
            TEXTURES.Add("T_Hills", Content.Load<Texture2D>("HillsTile"));
            TEXTURES.Add("T_Mountains", Content.Load<Texture2D>("MountainsTile"));
            TEXTURES.Add("T_Pasture", Content.Load<Texture2D>("PastureTile"));
            TEXTURES.Add("T_Water", Content.Load<Texture2D>("WaterTile"));
            TEXTURES.Add("TO_Number", Content.Load<Texture2D>("NumberTile"));

            base.LoadContent();

            startScreen = new MapScreen(state);

            screenManager.AddScreen("start", startScreen);
            //screenManager.SetScreen("start");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
