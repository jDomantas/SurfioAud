using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurfioAud.Geometry
{
    struct Triangle
    {
        public Vector A { get; }
        public Vector B { get; }
        public Vector C { get; }

        public Triangle(Vector a, Vector b, Vector c)
        {
            if ((b - a).Cross(c - b) < 0)
            {
                A = a;
                B = c;
                C = b;
            }
            else
            {
                A = a;
                B = b;
                C = c;
            }
        }

        public Triangle(Triangle from, Vector offset) : this(from.A + offset, from.B + offset, from.C + offset)
        {

        }

        public bool IntersectsCircle(Vector center, double radius)
        {
            return ContainsPoint(center)
                || CircleLine(A, B, center, radius)
                || CircleLine(B, C, center, radius)
                || CircleLine(C, A, center, radius);
        }

        public bool ContainsPoint(Vector x)
        {
            return (B - A).Cross(x - A) >= 0 && (C - B).Cross(x - B) >= 0 && (A - C).Cross(x - C) >= 0;
        }

        private bool CircleLine(Vector a, Vector b, Vector c, double r)
        {
            double d = (b - a).Dot(c - a) / (b - a).Length;
            if (d < 0) d = 0;
            if (d > 1) d = 1;
            return (c - (a + (b - a) * d)).LengthSquared <= r * r;
        }
        
        public void DrawDebug(SpriteBatch sb, Vector camera)
        {
            DrawLine(sb, A - camera, B - camera);
            DrawLine(sb, B - camera, C - camera);
            DrawLine(sb, C - camera, A - camera);
        }

        private static void DrawLine(SpriteBatch sb, Vector a, Vector b)
        {
            a.Y *= -1;
            b.Y *= -1;
            double angle = Math.Atan2(b.Y - a.Y, b.X - a.X);
            double len = (b - a).Length;
            sb.Draw(Resources.Pixel, new Rectangle((int)a.X, (int)a.Y, (int)len, 2), null, Color.Red, (float)angle, new Vector2(0, 0.5f), SpriteEffects.None, 0);
        }
    }
}
