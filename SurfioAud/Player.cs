using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private bool _wasInWater;
        private bool _snapped;
        private double _keepSnap;
        private bool _alive;
        private double _wobbleTimer;

        public double CollisionRadius => 52 + (11 + Math.Min(0, _currentFrame)) * 0.92;
        public Vector CollisionCenter => _position + new Vector(0, (15 + Math.Min(0, _currentFrame)) * 3);

        public Player(Vector position)
        {
            _position = position;
            _velocity = Vector.Zero;
            _angle = 0;
            _currentFrame = 0;
            _frameTimeLeft = 0;
            _wasInWater = false;
            _snapped = true;
            _alive = true;
        }

        public void Update(double dt, IWave wave)
        {
            if (!_alive)
            {
                return;
            }

            double prev = wave.GetHeight(_position.X - HalfInterval);
            double next = wave.GetHeight(_position.X + HalfInterval);
            double water = wave.GetHeight(_position.X);

            bool isInWater = _position.Y < water;

            if (!_snapped)
            {
                double rot = dt * 3;
                if (_angle < -rot && _wobbleTimer <= 0)
                {
                    _angle += rot;
                }
                else if (_angle > rot && _wobbleTimer <= 0)
                {
                    _angle -= rot;
                }
                else
                {
                    _wobbleTimer += dt * 13;
                    _angle = Math.Sin(_wobbleTimer) * 0.16;
                }
            }
            else
            {
                _wobbleTimer = 0;
            }

            if (_keepSnap >= 0)
            {
                _keepSnap -= dt;
            }

            if (_snapped && prev < next - 80 && _keepSnap <= 0)
            {
                _snapped = false;
            }

            if (_snapped)
            {
                double newVelY = (water - _position.Y) / dt;
                if (newVelY < _velocity.Y - 80 && _keepSnap <= 0)
                {
                    _snapped = false;
                    _keepSnap = 0.15;
                }
                else
                {
                    _velocity.Y = newVelY;
                }

                if (prev > next)
                {
                    _angle = Math.Atan2(prev - next, HalfInterval * 2);
                    double alpha = (prev - next) / (2 * HalfInterval);
                    double accelX = 1000 * alpha / (alpha * alpha + 1);
                    _velocity.X += dt * accelX;
                }
            }
            else
            {
                if (isInWater != _wasInWater)
                {
                    if (!_wasInWater)
                    {
                        wave.MakeSplash(_position.X);
                    }
                    _velocity.Y /= 3;
                    _wasInWater = isInWater;
                }

                if (_position.Y < water - 20)
                {
                    _velocity.Y += dt * 1300;
                    _velocity.Y /= Math.Pow(1.2, dt);
                }
                else if (_position.Y > water + 20)
                {
                    _velocity.Y -= dt * 1000;
                }

                if (_velocity.Y > 0 && Math.Abs(water - _position.Y) < 10 && _wasInWater && _keepSnap <= 0)
                {
                    _position.Y = water;
                    _velocity.Y = 0;
                    _snapped = true;
                    _keepSnap = 0.3;
                }
            }
            
            double reduce = Math.Abs(_velocity.X) * dt * 0.45;
            if (!_snapped && _position.Y > water)
            {
                reduce = 0;
            }
 
            if (_velocity.X > reduce)
            {
                _velocity.X -= reduce;
            }
            else if (_velocity.X < -reduce)
            {
                _velocity.X += reduce;
            }
            else
            {
                _velocity.X = 0;
            }

            _position += _velocity * dt;
            
            int targetFrame = Math.Min(15, Math.Max(-15, (int)Math.Round(_angle * 15 * 1.2)));
            if (!_snapped)
            {
                targetFrame = -15;
            }
            if (targetFrame < -4)
            {
                targetFrame += 4;
            }
            else if (targetFrame > 4)
            {
                targetFrame -= 4;
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

            // _currentFrame = -11;
        }

        public void Kill()
        {
            _alive = false;
        }

        public void Draw(SpriteBatch sb, Vector camera)
        {
            if (!_alive)
            {
                return;
            }

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
            if (Game1.DebugInfo)
            {
                sb.Draw(Resources.Pixel, new Rectangle(x - 5, y - 5, 10, 10), Color.Red);
                x = (int)Math.Round(CollisionCenter.X - camera.X);
                y = (int)Math.Round(camera.Y - CollisionCenter.Y);
                sb.Draw(Resources.Circle, new Rectangle(x - (int)CollisionRadius, y - (int)CollisionRadius, 2 * (int)CollisionRadius, 2 * (int)CollisionRadius), Color.Red);
            }
        }
    }
}
