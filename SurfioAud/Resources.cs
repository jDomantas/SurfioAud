using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SurfioAud
{
    class Resources
    {
        public static Texture2D Pixel;
        public static Texture2D Circle;
        public static Texture2D[] Player = new Texture2D[4];
        public static Texture2D[] PlayerForward = new Texture2D[4];
        public static Texture2D[] PlayerBackward = new Texture2D[4];
        public static Texture2D PlayerDeath;
        public static Texture2D Background;
        public static Texture2D Paralax1;
        public static Texture2D Paralax2;
        public static Texture2D Static;

        public static List<Obstacle> Obstacles = new List<Obstacle>();

        public static SpriteFont Font;

        public static SoundEffect StaticNoise;
    }
}
