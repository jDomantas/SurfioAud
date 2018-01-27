﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurfioAud.Waves;
using System;

namespace SurfioAud
{
    class Player
    {
        private const double HalfInterval = 30;

        public Vector Position => _position;
        private Vector _position;
        private Vector _velocity;

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

            _position.Y = water;
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

            double reduce = Math.Abs(_velocity.X) * dt * 0.75;
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
