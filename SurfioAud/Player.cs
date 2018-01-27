using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurfioAud.Waves;
using System;

namespace SurfioAud
{
    class Player
    {
        private const double HalfInterval = 10;

        public Vector Position => _position;
        private Vector _position;
        private Vector _velocity;
        private bool _wasInWater;

        public Player(Vector position)
        {
            _position = position;
            _velocity = Vector.Zero;
        }

        public void Update(double dt, IWave wave)
        {
            double prev = wave.GetHeight(_position.X - HalfInterval);
            double next = wave.GetHeight(_position.X + HalfInterval);
            double water = (prev + next) / 2;

            bool isInWater = _position.Y < water;
            isInWater = false;
            _position.Y = water;
            _velocity.Y = 0;
            if (isInWater != _wasInWater)
            {
                _velocity.Y /= 2;
            }

            if (_position.Y < water - 20)
            {
                _velocity.Y += dt * 1000;
                _velocity.Y /= Math.Pow(1.2, dt);
            }
            else if (_position.Y > water + 20)
            {
                _velocity.Y -= dt * 1000;
            }
            else if (prev > next)
            {
                double k = dt * 20;
                double t = (prev - next) * dt * 1000;
                if (t > _velocity.X)
                {
                    _velocity.X = _velocity.X * (1 - k) + k * t;
                }
            }

            double reduce = 50 * dt;
            if (_velocity.X > reduce)
            {
                _velocity.X -= reduce;
            }
            else if (_velocity.X < reduce)
            {
                _velocity.X += reduce;
            }
            else
            {
                _velocity.X = 0;
            }
            //_velocity /= Math.Pow(1.6, dt);
            _position += _velocity * dt;

            _wasInWater = isInWater;
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
