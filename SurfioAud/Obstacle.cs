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

        public Obstacle(bool isLong, bool isTop, Vector offset, Texture2D texture, Polygon collisionBox)
        {
            IsLong = isLong;
            IsTop = isTop;
            _textureOffset = offset;
            _texture = texture;
            _collisionBox = collisionBox;
        }

        public Obstacle(Obstacle from, Vector offset)
        {
            IsLong = from.IsLong;
            IsTop = from.IsTop;
            _textureOffset = from._textureOffset + offset;
            _texture = from._texture;
            _collisionBox = new Polygon(from._collisionBox, offset);
        }

        public bool IntersectsCircle(Vector center, double radius)
        {
            return _collisionBox.IntersectsCircle(center, radius);
        }

        public void Draw(SpriteBatch sb, Vector camera)
        {
            int x = (int)Math.Round(_textureOffset.X - camera.X);
            int y = (int)Math.Round(camera.Y - _textureOffset.Y);
            sb.Draw(_texture, new Vector2(x, y), Color.White);
            if (Game1.DebugInfo)
            {
                _collisionBox.DrawDebug(sb, camera);
            }
        }
    }
}
