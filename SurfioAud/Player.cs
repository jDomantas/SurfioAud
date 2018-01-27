using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurfioAud.Waves;
using System;

namespace SurfioAud
{
    class Player
    {
        public Vector Position { get; private set; }
        private Vector _velocity;

        public Player(Vector position)
        {
            Position = position;
            _velocity = Vector.Zero;
        }

        public void Update(double dt, IWave wave)
        {
            Position = new Vector(Position.X, wave.GetHeight(Position.X));
        }

        public void Draw(SpriteBatch sb, Vector camera)
        {
            Vector screenPos = Position - camera;
            int x = (int)Math.Round(screenPos.X);
            int y = (int)Math.Round(-screenPos.Y);
            sb.Draw(Resources.Pixel, new Rectangle(x - 10, y - 10, 20, 20), Color.Black);
        }
    }
}
