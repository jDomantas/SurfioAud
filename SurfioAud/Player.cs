﻿using Microsoft.Xna.Framework;
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
        private double _blinkTimer;
        private double _idleThingTimer;
        private double _deathTime;

        private static readonly Random Rnd = new Random();

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
            
            _blinkTimer = -(Rnd.NextDouble() + 1) * 1;
            _idleThingTimer = -2;
        }

        public void Update(double dt, IWave wave)
        {
            if (!_alive)
            {
                _deathTime += dt;
                return;
            }

            if (_idleThingTimer > 0)
            {
                _idleThingTimer -= dt;
                if (_idleThingTimer <= 0)
                {
                    _idleThingTimer = -(Rnd.NextDouble() + 0.5) * 4;
                }
            }
            else
            {
                if (_currentFrame != 0)
                {
                    _idleThingTimer = -Math.Max(3, -_idleThingTimer);
                }
                _idleThingTimer += dt;
                if (_idleThingTimer >= 0)
                {
                    _idleThingTimer = 2;
                }
            }

            if (_blinkTimer > 0)
            {
                _blinkTimer -= dt;
                if (_blinkTimer <= 0)
                {
                    if (Rnd.Next(5) == 0)
                    {
                        _blinkTimer = -0.3;
                    }
                    else
                    {
                        _blinkTimer = -(Rnd.NextDouble() + 0.5) * 3;
                    }
                }
            }
            else
            {
                _blinkTimer += dt;
                if (_blinkTimer >= 0)
                {
                    _blinkTimer = 0.02 * 5;
                }
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
            Vector screenPos = Position - camera;
            int x = (int)Math.Round(screenPos.X);
            int y = (int)Math.Round(-screenPos.Y);
            //sb.Draw(Resources.Pixel, new Rectangle(x - 10, y - 10, 20, 20), Color.Black);
            int sx, sy;
            Texture2D tex;
            int subtex = 0;
            if (_blinkTimer > 0)
            {
                subtex = (int)Math.Floor(_blinkTimer / 0.02);
                subtex = Math.Min(4, Math.Max(0, subtex));
                if (subtex > 2) subtex = 2 - (subtex - 2);
                subtex++;
            }
            if (_currentFrame == 0)
            {
                sx = 0;
                sy = 0;
                tex = Resources.Player[subtex];

                if (_idleThingTimer > 0)
                {
                    int part = (int)Math.Floor(_idleThingTimer / 0.2);
                    if (part < 0) part = 0;
                    if (part > 9) part = 9;
                    int frame = 0;
                    if (part == 0) frame = 1;
                    else if (part == 1) frame = 2;
                    else if (part >= 2 && part <= 7) frame = 3;
                    else if (part == 8) frame = 2;
                    else frame = 1;
                    sx = 250 * (frame % 2);
                    sy = 250 * (frame / 2);
                }
            }
            else if (_currentFrame > 0)
            {
                sx = 250 * (_currentFrame % 4);
                sy = 250 * (_currentFrame / 4);
                tex = Resources.PlayerBackward[subtex];
            }
            else
            {
                sx = 250 * (-_currentFrame % 4);
                sy = 250 * (-_currentFrame / 4);
                tex = Resources.PlayerForward[subtex];
            }
            if (!_alive)
            {
                int frame = (int)Math.Floor(_deathTime / 0.04);
                if (frame >= 8) return;
                tex = Resources.PlayerDeath;
                sx = frame % 4 * 250;
                sy = frame / 4 * 250;
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
