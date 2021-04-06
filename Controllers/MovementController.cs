using CoreAppGame.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CoreAppGame.Controllers
{
    public class MovementController
    {
        public InputKeys InputKeys { get; set; }
        
        public MovementController()
        {
            InputKeys = new InputKeys()
            {
                // For directions

                Up = Keys.Up,
                Down = Keys.Down,
                Left = Keys.Left,
                Right = Keys.Right,

                // Rotate and Move

                TurnLeft = Keys.A,
                TurnRight = Keys.D,
                Forward = Keys.Space
            };
        }

        public Vector2 ChangeDirection(KeyboardState keyState)
        {
            Vector2 Direction = Vector2.Zero;

            if (keyState.IsKeyDown(InputKeys.Right))
            {
                Direction = new Vector2(1, 0);
            }

            if (keyState.IsKeyDown(InputKeys.Left))
            {
                Direction = new Vector2(-1, 0);
            }

            if (keyState.IsKeyDown(InputKeys.Up))
            {
                Direction = new Vector2(0, -1);
            }

            if (keyState.IsKeyDown(InputKeys.Down))
            {
                Direction = new Vector2(0, 1);
            }

            return Direction;
        }

        public Vector2 ChangeDirection()
        {
            Vector2 Direction = Vector2.Zero;

            GamePadDPad dPad = GamePad.GetState(PlayerIndex.One).DPad;

            if(dPad.Down == ButtonState.Pressed)
            {
                Direction = new Vector2(0, 1);
            }
            else if (dPad.Up == ButtonState.Pressed)
            {
                Direction = new Vector2(0, -1);
            }
            else if (dPad.Left == ButtonState.Pressed)
            {
                Direction = new Vector2(-1, 0);
            }
            else if (dPad.Right == ButtonState.Pressed)
            {
                Direction = new Vector2(1, 0);
            }

            return Direction;
        }

    }
}
