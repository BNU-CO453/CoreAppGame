using CoreAppGame.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoreAppGame.Controllers
{
    /// <summary>
    /// This class takes a sprite sheet with one row
    /// of many images and cycles through it frame
    /// by frame returning the current frame in
    /// the update method as a source Rectangle
    /// </summary>
    /// <authors>
    /// Derek Peacock & Andrei Cruceru
    /// </authors>
    public class Animation
    {
        public string Name { get; private set; }
        // One animation: one row of multiple images
        public Texture2D FrameSet { get; set; }
        public int CurrentFrame { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsLooping { get; set; }
        public int FramesPerSecond { get; set; }

        private readonly int NumberOfFrames;
        private readonly int frameWidth;
        private readonly int frameHeight;
        
        private float elapsedTime;
        private float maxFrameTime;

        public Animation(string name, Texture2D frameSet, int frames)
        {
            Name = name;
            FrameSet = frameSet;
            NumberOfFrames = frames;

            FramesPerSecond = 10;
            frameHeight = FrameSet.Height;
            frameWidth = FrameSet.Width / NumberOfFrames;
            IsLooping = true;

            Start();
        }

        public void Start()
        {
            CurrentFrame = NumberOfFrames - 1;
            IsPlaying = true;
            maxFrameTime = 1.0f / (float)FramesPerSecond;
            elapsedTime = 0;
        }

        public void Stop()
        {
            IsPlaying = false;
            maxFrameTime = 0;
            elapsedTime = 0;
        }


        public Texture2D SetMainFrame(GraphicsDevice graphics)
        {
            Texture2D mainFrame = FrameSet.CreateTexture(
                    graphics, new Rectangle(0, 0, frameWidth, frameHeight));

            return mainFrame;
        }

        public Rectangle Update(GameTime gameTime)
        {            
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(IsPlaying && elapsedTime >= maxFrameTime)
            {
                if (CurrentFrame < NumberOfFrames - 1)
                    CurrentFrame++;

                else if(IsLooping)
                    CurrentFrame = 0;

                elapsedTime = 0;
                
                return new Rectangle((CurrentFrame) * frameWidth, 0,
                    frameWidth, frameHeight);
            }
            // this will return the previous frame instead of an empty rectangle
            //// so we are always drawing an image.
            return new Rectangle((CurrentFrame) * frameWidth, 0,
                    frameWidth, frameHeight);
        }        
    }
}
