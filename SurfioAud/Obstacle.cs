using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurfioAud.Geometry;
using System;

namespace SurfioAud
{
    class Obstacle
    {
        private readonly Vector _textureOffset;
        private readonly Texture2D _texture;
        private readonly Polygon _collisionBox;

        public double Left => _collisionBox.Left;
        public double Right => _collisionBox.Right;
        public bool IsLong { get; }
        public bool IsTop { get; }

        private bool _vflip;

        public Obstacle(bool isLong, bool isTop, Vector offset, Texture2D texture, Polygon collisionBox)
        {
            IsLong = isLong;
            IsTop = isTop;
            _textureOffset = offset;
            _texture = texture;
            _collisionBox = collisionBox;
            _vflip = false;
        }

        public Obstacle(Obstacle from, Vector offset)
        {
            IsLong = from.IsLong;
            IsTop = from.IsTop;
            _textureOffset = from._textureOffset + offset;
            _texture = from._texture;
            _collisionBox = new Polygon(from._collisionBox, offset);
            _vflip = from._vflip;
        }

        public Obstacle VFlipped()
        {
            _vflip = true;
            return this;
        }

        public bool IntersectsCircle(Vector center, double radius)
        {
            return _collisionBox.IntersectsCircle(center, radius);
        }

        public void Draw(SpriteBatch sb, Vector camera)
        {
            int x = (int)Math.Round(_textureOffset.X - camera.X);
            int y = (int)Math.Round(camera.Y - _textureOffset.Y);
            sb.Draw(_texture, new Rectangle(x, y, _texture.Width, _texture.Height), null, Color.White, 0, Vector2.Zero, _vflip ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
            if (Game1.DebugInfo)
            {
                _collisionBox.DrawDebug(sb, camera);
            }
        }
    }
}
