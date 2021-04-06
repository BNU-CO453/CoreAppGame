using CoreAppGame.Controllers;
using CoreAppGame.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CoreAppGame
{
    public enum GameStates
    {
        Start,
        Playing,
        End
    }

    /// <summary>
    /// 
    /// </summary>
    public class CoinChaseGame : Game
    {

        public static Vector2 UHD = new Vector2(3840, 2160);
        public static Vector2 HD = new Vector2(1920, 1080);
        public static Vector2 P720 = new Vector2(1280, 720);

        public int Screen_Height = (int)UHD.Y;
        public int Screen_Width = (int)UHD.X;

        public const string MUSIC = "Adventure";
        public const string TAKE_COIN_SOUND = "Coins";
        public const string DIE_SOUND = "scream01";

        private Random generator = new Random();

        private GraphicsDeviceManager graphicsManager;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;

        private Texture2D backgroundImage;

        private CoinsController coinsController;

        private AnimatedPlayer playerSprite;
        private AnimatedSprite enemySprite;
        private SoundEffect screamEffect;

        private SpriteFont arialFont;
        private SpriteFont calibriFont;

        private GameStates gameState;
        private Vector2 startPosition;

        private Rectangle gameBounds;

        private float scale = 2.0f;
        private int speed = 200;
        public CoinChaseGame()
        {
            graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;


        }

        protected override void Initialize()
        {
            graphicsManager.PreferredBackBufferWidth = Screen_Width;
            graphicsManager.PreferredBackBufferHeight = Screen_Height;

            graphicsManager.ApplyChanges();

            graphicsDevice = graphicsManager.GraphicsDevice;

            gameState = GameStates.Start;
            startPosition = new Vector2(Screen_Width / 2, Screen_Height / 2);

            if(Screen_Width == UHD.X)
            {
                scale = 4.0f;
                speed = 400;
            }

            int y1 = Screen_Height * 36 / 100;
            int h1 = (Screen_Height * 81 / 100) - y1;

            gameBounds = new Rectangle(0, y1, Screen_Width, h1);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundImage = Content.Load<Texture2D>(
                "game_background_4");
            
            // Load Music and SoundEffects

            SoundController.LoadContent(Content);
            SoundController.PlaySong(MUSIC, true);

            screamEffect = SoundController.GetSoundEffect("Scream");

            // Load Fonts

            arialFont = Content.Load<SpriteFont>("fonts/arial");
            calibriFont = Content.Load<SpriteFont>("fonts/calibri");

            // animated sprites suitable for pacman type game

            coinsController = new CoinsController(graphicsDevice, Content);
            coinsController.Boundary = gameBounds;
            coinsController.CreateCoin();

            SetupAnimatedPlayer();
            SetupEnemy();

            gameState = GameStates.Playing;
        }

        /// <summary>
        /// This is a Sprite with four animations for the four
        /// directions, up, down, left and right
        /// </summary>
        private void SetupAnimatedPlayer()
        {
            Texture2D sheet4x3 = Content.Load<Texture2D>("rsc-sprite-sheet1");

            AnimationController manager = new AnimationController(graphicsDevice, sheet4x3, 4, 3);

            string[] keys = new string[] { "Down", "Left", "Right", "Up" };
            manager.CreateAnimationGroup(keys);

            playerSprite = new AnimatedPlayer()
            {
                CanWalk = true,
                Scale = scale,

                Position = startPosition,
                Speed = speed,
                Direction = new Vector2(1, 0),

                Rotation = MathHelper.ToRadians(0),
                RotationSpeed = 0f,

                Boundary = gameBounds
            };

            manager.AppendAnimationsTo(playerSprite);
        }

        /// <summary>
        /// This is an enemy Sprite with four animations for the four
        /// directions, up, down, left and right.  Has no intelligence!
        /// </summary>
        private void SetupEnemy()
        {
            Texture2D sheet4x3 = Content.Load<Texture2D>("rsc-sprite-sheet3");

            AnimationController manager = new AnimationController(graphicsDevice, sheet4x3, 4, 3);

            string[] keys = new string[] { "Down", "Left", "Right", "Up" };

            manager.CreateAnimationGroup(keys);

            enemySprite = new AnimatedSprite()
            {
                Scale = scale,

                Position = new Vector2(Screen_Width - 200, Screen_Height / 2),
                Direction = new Vector2(-1, 0),
                Speed = speed,

                Rotation = MathHelper.ToRadians(0),
                Boundary = gameBounds
            };

            manager.AppendAnimationsTo(enemySprite);
            enemySprite.PlayAnimation("Left");
        }

        /// <summary>
        /// Called 60 frames/per second and updates the positions
        /// of all the drawable objects
        /// </summary>
        /// <param name="gameTime">
        /// Can work out the elapsed time since last call if
        /// you want to compensate for different frame rates
        /// </param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            if (gameState == GameStates.Playing)
            {
                // Update Chase Game

                playerSprite.Update(gameTime);
                enemySprite.Update(gameTime);

                if (!playerSprite.IsActive && playerSprite.Health < 100)
                {
                    playerSprite.Health += gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (playerSprite.HasCollided(enemySprite) || playerSprite.Health <= 0)
                {
                    screamEffect.Play();

                    playerSprite.IsActive = false;
                    playerSprite.IsAlive = false;
                    playerSprite.IsVisible = false;


                    enemySprite.IsActive = false;
                    SoundController.PauseSong();
                    gameState = GameStates.End;
                }

                coinsController.Update(gameTime);
                coinsController.HasCollided(playerSprite);

                if (enemySprite.Position.X < enemySprite.Width/2)
                {
                    int x = (int)enemySprite.Position.X;

                    int y = generator.Next(gameBounds.Y,
                        gameBounds.Y + gameBounds.Height - enemySprite.Height);

                    enemySprite.Direction = new Vector2(1, 0);
                    enemySprite.PlayAnimation("Right");
                    enemySprite.Position = new Vector2(x + enemySprite.Width, y);
                }
                else if(enemySprite.Position.X > gameBounds.Width - (enemySprite.Width + 20))
                {
                    int x = (int)enemySprite.Position.X;

                    int y = generator.Next(gameBounds.Y,
                        gameBounds.Y + gameBounds.Height - enemySprite.Height);

                    enemySprite.Direction = new Vector2(-1, 0);
                    enemySprite.PlayAnimation("Left");
                    enemySprite.Position = new Vector2(x - enemySprite.Width, y);
                }
            }
            else if (gameState == GameStates.End)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed
                    || Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Restart();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Reactivate the player and put them back to the
        /// start position, clear the scores and coins
        /// </summary>
        private void Restart()
        {
            gameState = GameStates.Playing;
            playerSprite.Position = startPosition;

            playerSprite.IsActive = true;
            playerSprite.IsAlive = true;
            playerSprite.IsVisible = true;

            playerSprite.Score = 0;
            playerSprite.Health = 100;
            coinsController.Clear();

            enemySprite.IsActive = true;
            enemySprite.Position = new Vector2(Screen_Width - 200, Screen_Height / 2);

            SoundController.PlaySong(MUSIC, true);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundImage, Vector2.Zero, Color.White);
            // Draw Chase game

            if (gameState == GameStates.Playing)
            {
                playerSprite.Draw(spriteBatch);
                coinsController.Draw(spriteBatch);
                enemySprite.Draw(spriteBatch);
            }
            else if (gameState == GameStates.End)
            {
                DrawGameEnd(spriteBatch);
            }

            DrawGameStatus(spriteBatch);
            DrawGameFooter(spriteBatch);


            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Display the name fo the game and the current score
        /// and health of the player at the top of the screen
        /// </summary>
        public void DrawGameStatus(SpriteBatch spriteBatch)
        {
            Vector2 topLeft = new Vector2(100, 4);
            string status = $"Score = {playerSprite.Score:##0}";

            spriteBatch.DrawString(arialFont, status, topLeft, Color.Blue);

            string game = "Coin Chase - for Ella & Lottie";
            Vector2 gameSize = arialFont.MeasureString(game);
            Vector2 topCentre = new Vector2((Screen_Width / 2 - gameSize.X / 2), 4);
            spriteBatch.DrawString(arialFont, game, topCentre, Color.Blue);
            string healthText = $"Health = {playerSprite.Health:0.00}%";
            Vector2 healthSize = arialFont.MeasureString(healthText);
            Vector2 topRight = new Vector2(Screen_Width - (healthSize.X + 100), 4);
            spriteBatch.DrawString(arialFont, healthText, topRight, Color.Blue);

        }

        public void DrawGameEnd(SpriteBatch spriteBatch)
        {
            string message = "You have just died!  Press (A) to restart!!!";
            Vector2 messageSize = arialFont.MeasureString(message);

            Vector2 centre = new Vector2(
                Screen_Width / 2 - messageSize.X / 2, Screen_Height / 2);

            spriteBatch.DrawString(arialFont, message, centre, Color.Red);
        }

        /// <summary>
        /// Display the Module, the authors and the application name
        /// at the bottom of the screen
        /// </summary>
        public void DrawGameFooter(SpriteBatch spriteBatch)
        {
            int margin = 40;

            string names = "Derek & Andrei";
            string app = "App05: Coin Chase";
            string module = "BNU CO453-2021";

            Vector2 namesSize = calibriFont.MeasureString(names);
            Vector2 appSize = calibriFont.MeasureString(app);

            Vector2 bottomCentre = new Vector2((Screen_Width - namesSize.X) / 2, Screen_Height - margin);
            Vector2 bottomLeft = new Vector2(margin, Screen_Height - margin);
            Vector2 bottomRight = new Vector2(Screen_Width - appSize.X - margin, Screen_Height - margin);

            spriteBatch.DrawString(calibriFont, names, bottomCentre, Color.Yellow);
            spriteBatch.DrawString(calibriFont, module, bottomLeft, Color.Yellow);
            spriteBatch.DrawString(calibriFont, app, bottomRight, Color.Yellow);

        }
    }
}
