using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Robot_Rampage
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D spriteSheet;
        Texture2D titleScreen;
        SpriteFont pericles14;

        enum GameStates { TitleScreen, Playing, WaveComplete, GameOver };
        GameStates gameState = GameStates.TitleScreen;

        float gameOverTimer = 0.0f;
        float gameOverDelay = 6.0f;

        float waveCompleteTimer = 0.0f;
        float waveCompleteDelay = 6.0f;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
          
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteSheet = Content.Load<Texture2D>(@"SpriteSheet");
            titleScreen = Content.Load<Texture2D>(@"TitleScreen");
            pericles14 = Content.Load<SpriteFont>(@"Pericles14");

            Camera.WorldRectangle = new Rectangle(0, 0, 1600, 1600);
            Camera.ViewPortWidth = 800;
            Camera.ViewPortHeight = 600;

            TileMap.Initialize(spriteSheet);

            Player.Initialize(
                spriteSheet,
                new Rectangle(0, 64, 32, 32),
                6,
                new Rectangle(0, 96, 32, 32),
                1,
                new Vector2(32, 32));

            EffectsManager.Initialize(
                spriteSheet,
                new Rectangle(0, 288, 2, 2),
                new Rectangle(0, 256, 32, 32),
                3);

            WeaponManager.Texture = spriteSheet;

            GoalManager.Initialize(
                spriteSheet,
                new Rectangle(0, 7 * 32, 32, 32),
                new Rectangle(3 * 32, 7 * 32, 32, 32),
                3,
                1);

            EnemyManager.Initialize(
                spriteSheet,
                new Rectangle(0, 160, 32, 32));



        }


        protected override void UnloadContent()
        {

        }

        private void checkPlayerDeath()
        {
            foreach (Enemy enemy in EnemyManager.Enemies)
            {
                if (enemy.EnemyBase.IsCircleColliding(
                    Player.BaseSprite.WorldCenter,
                    Player.BaseSprite.CollisionRadius))
                {
                    gameState = GameStates.GameOver;
                }
            }
        }

       
        protected override void Update(GameTime gameTime)
        {
           
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed)
                this.Exit();

            switch (gameState)
            {
                case GameStates.TitleScreen:
                    if ((GamePad.GetState(PlayerIndex.One).Buttons.A ==
                        ButtonState.Pressed) ||
                        (Keyboard.GetState().IsKeyDown(Keys.Space)))
                    {
                        GameManager.StartNewGame();
                        gameState = GameStates.Playing;
                    }
                    break;

                case GameStates.Playing:
                    Player.Update(gameTime);
                    WeaponManager.Update(gameTime);
                    EnemyManager.Update(gameTime);
                    EffectsManager.Update(gameTime);
                    GoalManager.Update(gameTime);
                    if (GoalManager.ActiveTerminals == 0)
                    {
                        gameState = GameStates.WaveComplete;
                    }
                    break;

                case GameStates.WaveComplete:
                    waveCompleteTimer +=
                        (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (waveCompleteTimer > waveCompleteDelay)
                    {
                        GameManager.StartNewWave();
                        gameState = GameStates.Playing;
                        waveCompleteTimer = 0.0f;
                    }
                    break;

                case GameStates.GameOver:
                    gameOverTimer +=
                        (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (gameOverTimer > gameOverDelay)
                    {
                        gameState = GameStates.TitleScreen;
                        gameOverTimer = 0.0f;
                    }
                    break;
            }

            base.Update(gameTime);
        }


      
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (gameState == GameStates.TitleScreen)
            {
                spriteBatch.Draw(
                    titleScreen,
                    new Rectangle(0, 0, 800, 600),
                    Color.White);
            }

            if ((gameState == GameStates.Playing) ||
                (gameState == GameStates.WaveComplete) ||
                (gameState == GameStates.GameOver))
            {
                TileMap.Draw(spriteBatch);
                WeaponManager.Draw(spriteBatch);
                Player.Draw(spriteBatch);
                EnemyManager.Draw(spriteBatch);
                EffectsManager.Draw(spriteBatch);
                GoalManager.Draw(spriteBatch);

                checkPlayerDeath();

                spriteBatch.DrawString(
                    pericles14,
                    "Score: " + GameManager.Score.ToString(),
                    new Vector2(30, 5),
                    Color.White);

                spriteBatch.DrawString(
                    pericles14,
                    "Terminals Remaining: " +
                        GoalManager.ActiveTerminals,
                    new Vector2(520, 5),
                    Color.White);
            }

            if (gameState == GameStates.WaveComplete)
            {
                spriteBatch.DrawString(
                    pericles14,
                    "Beginning Wave " +
                        (GameManager.CurrentWave + 1).ToString(),
                    new Vector2(300, 300),
                    Color.White);
            }

            if (gameState == GameStates.GameOver)
            {
                spriteBatch.DrawString(
                    pericles14,
                    "G A M E O V E R!",
                    new Vector2(300, 300),
                    Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}