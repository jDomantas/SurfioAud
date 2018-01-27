using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurfioAud.Waves;
using System;

namespace SurfioAud
{
    class Player
    {
        private const double HalfInterval = 40;
        private const double FrameTime = 0.02;

        public Vector Position => _position;
        private Vector _position;
        private Vector _velocity;
        private double _angle;
        private double _frameTimeLeft;
        private int _currentFrame;

        public Player(Vector position)
        {
            _position = position;
            _velocity = Vector.Zero;
            _angle = 0;
            _currentFrame = 0;
            _frameTimeLeft = 0;
        }

        public void Update(double dt, IWave wave)
        {
            double prev = wave.GetHeight(_position.X - HalfInterval);
            double next = wave.GetHeight(_position.X + HalfInterval);
            double water = (prev + next) / 2;
            _angle = Math.Atan2(prev - next, HalfInterval * 2);

            _position.Y = wave.GetHeight(_position.X);
            _velocity.Y = 0;

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
                double alpha = (prev - next) / (2 * HalfInterval);
                double accel = 1000 * alpha / (alpha * alpha + 1);
                _velocity.X += dt * accel;

            }

            double reduce = Math.Abs(_velocity.X) * dt * 0.45;
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
            
            _position += _velocity * dt;

            int targetFrame = Math.Min(13, Math.Max(-13, (int)Math.Round(_angle * 13)));
            if (targetFrame < -2)
            {
                targetFrame += 2;
            }
            else if (targetFrame > 2)
            {
                targetFrame -= 2;
            }
            else
            {
                targetFrame = 0;
            }

            _frameTimeLeft -= dt;
            if (_frameTimeLeft <= 0)
            {
                _frameTimeLeft = 0;
                if (targetFrame < _currentFrame)
                {
                    _frameTimeLeft = FrameTime;
                    _currentFrame--;
                }
                else if (targetFrame > _currentFrame)
                {
                    _frameTimeLeft = FrameTime;
                    _currentFrame++;
                }
            }
        }

        public void Draw(SpriteBatch sb, Vector camera)
        {
            Vector screenPos = Position - camera;
            int x = (int)Math.Round(screenPos.X);
            int y = (int)Math.Round(-screenPos.Y);
            //sb.Draw(Resources.Pixel, new Rectangle(x - 10, y - 10, 20, 20), Color.Black);
            int sx, sy;
            Texture2D tex;
            if (_currentFrame == 0)
            {
                sx = 0;
                sy = 0;
                tex = Resources.Player;
            }
            else if (_currentFrame > 0)
            {
                sx = 250 * (_currentFrame % 4);
                sy = 250 * (_currentFrame / 4);
                tex = Resources.PlayerBackward;
            }
            else
            {
                sx = 250 * (-_currentFrame % 4);
                sy = 250 * (-_currentFrame / 4);
                tex = Resources.PlayerForward;
            }
            sb.Draw(tex, new Rectangle(x, y, 250, 250), new Rectangle(sx, sy, 250, 250), Color.White, (float)_angle, new Vector2(125, 185), SpriteEffects.None, 0);
        }
    }
}
