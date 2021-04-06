using CoreAppGame.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace CoreAppGame.Controllers
{
    public enum CoinColours
    {
        Copper = 100,
        Silver = 200,
        Gold = 500
    }

    /// <summary>
    /// This class creates a list of coins which
    /// can be updated and drawn and checked for
    /// collisions with the player sprite
    /// </summary>
    /// <authors>
    /// Derek Peacock & Andrei Cruceru
    /// </authors>
    public class CoinsController
    {
        public Rectangle Boundary { get; set; }
        public int NoCollected { get; set; }


        private Random generator = new Random();

        private SoundEffect coinEffect;

        private readonly List<AnimatedSprite> Coins;

        private float maxTime = 3f;

        private float elapsedTime;

        private List<Texture2D> colours;

        private GraphicsDevice graphics;

        private ContentManager content;

        AnimatedSprite FirstCoin;

        public CoinsController(GraphicsDevice graphics, ContentManager content)
        {
            this.graphics = graphics;
            this.content = content;

            colours = new List<Texture2D>();
            Coins = new List<AnimatedSprite>();

            Texture2D coinSheet = content.Load<Texture2D>("coin_copper");
            AddColour(coinSheet);

            coinSheet = content.Load<Texture2D>("coin_silver");
            AddColour(coinSheet);

            coinSheet = content.Load<Texture2D>("coin_gold");
            AddColour(coinSheet);
        }

        public void AddColour(Texture2D sheet)
        {
            colours.Add(sheet);
        }
        
        /// <summary>
        /// Create an animated sprite of a copper coin
        /// which could be collected by the player for a score
        /// </summary>
        public void CreateCoin()
        {
            int index = 0;

            if(colours.Count > 1)
            {
                index = generator.Next(colours.Count);
            }


            coinEffect = SoundController.GetSoundEffect("Coin");

            Animation animation = new Animation("coin", colours[index], 8);

            float scale = 2.0f;


            if (graphics.Viewport.Width > 2000)
                scale = 4.0f;

            AnimatedSprite coin = new AnimatedSprite()
            {
                Animation = animation,
                Image = animation.SetMainFrame(graphics),

                Scale = scale,
                Speed = 0,
            };

            switch(index)
            {
                case 0: coin.Colour = CoinColours.Copper; break;
                case 1: coin.Colour = CoinColours.Silver; break;
                case 2: coin.Colour = CoinColours.Gold; break;
            };

            int x = generator.Next(80, Boundary.Width - 80);
            int y = generator.Next(Boundary.Y, Boundary.Y + Boundary.Height - 80);

            coin.Position = new Vector2(x, y);

            elapsedTime = maxTime;
            Coins.Add(coin);
            FirstCoin = coin;
        }

        public void HasCollided(AnimatedPlayer player)
        {
            foreach (AnimatedSprite coin in Coins)
            {
                if (coin.HasCollided(player) && coin.IsAlive)
                {
                    coinEffect.Play();

                    coin.IsActive = false;
                    coin.IsAlive = false;
                    coin.IsVisible = false;

                    NoCollected++;

                    player.Score += (int)coin.Colour;
                }
            }           
        }

        public void Update(GameTime gameTime)
        {
            foreach(AnimatedSprite coin in Coins)
            {
                coin.Update(gameTime);
            }

            elapsedTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if(elapsedTime <= 0)
            {
                CreateCoin();
                elapsedTime = maxTime;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (AnimatedSprite coin in Coins)
            {
                coin.Draw(spriteBatch);
            }
        }

        public void Clear()
        {
            Coins.Clear();
        }
    }
}
