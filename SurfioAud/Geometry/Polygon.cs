using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace SurfioAud.Geometry
{
    class Polygon
    {
        private readonly List<Triangle> _triangles;
        public double Left { get; }
        public double Right { get; }

        public Polygon(params Vector[] points)
        {
            _triangles = new List<Triangle>();
            var pts = points.ToList();

            // very stupid triangulation algorithm
            while (pts.Count >= 3)
            {
                for (int i = 0; i < pts.Count; i++)
                {
                    Vector a = pts[(i - 1 + pts.Count) % pts.Count];
                    Vector b = pts[i];
                    Vector c = pts[(i + 1) % pts.Count];
                    if ((b - a).Cross(c - b) >= 0)
                    {
                        var triangle = new Triangle(a, b, c);
                        bool ok = true;
                        for (int j = 0; j < pts.Count; j++)
                        {
                            if (j == i) continue;
                            if (j == (i + 1) % pts.Count) continue;
                            if (j == (i - 1 + pts.Count) % pts.Count) continue;
                            if (triangle.ContainsPoint(pts[j]))
                            {
                                ok = false;
                                break;
                            }
                        }
                        if (ok)
                        {
                            _triangles.Add(new Triangle(a, b, c));
                            pts.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            Left = points.Min(p => p.X);
            Right = points.Max(p => p.X);
        }

        public Polygon(Polygon from, Vector offset)
        {
            _triangles = new List<Triangle>(from._triangles.Count);
            for (int i = 0; i < from._triangles.Count; i++)
            {
                _triangles.Add(new Triangle(from._triangles[i], offset));
            }

            Left = from.Left + offset.X;
            Right = from.Right + offset.X;
        }

        public bool IntersectsCircle(Vector center, double radius)
        {
            return _triangles.Any(t => t.IntersectsCircle(center, radius));
        }

        public void DrawDebug(SpriteBatch sb, Vector camera)
        {
            for (int i = 0; i < _triangles.Count; i++)
            {
                _triangles[i].DrawDebug(sb, camera);
            }
        }
    }
}
