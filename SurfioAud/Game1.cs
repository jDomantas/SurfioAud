using Microsoft.Xna.Framework;  
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurfioAud.Geometry;
using SurfioAud.Waves;
using System;
using System.Collections.Generic;

namespace SurfioAud
{
    public class Game1 : Game
    {
        public const bool DebugInfo = false;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly int[] _randomNumbers;

        private IWave _waves;
        private Player _player;
        private RenderTarget2D _renderTarget;
        private RenderTarget2D _waveRenderTarget;
        private int _tick;
        private List<Obstacle> _obstacles;
        private double _nextTopSlot;
        private double _nextBotSlot;
        private bool _forceTopLong;
        private bool _forceBotLong;
        private int _deathTimer;
        private readonly Random _rnd = new Random(0);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            _randomNumbers = new int[1920 * 10];
        }
 
        protected override void Initialize()
        {
            base.Initialize();

            _graphics.IsFullScreen = false;
            if (_graphics.IsFullScreen)
            {
                _graphics.PreferredBackBufferWidth = 1920;
                _graphics.PreferredBackBufferHeight = 1080;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = 1920 * 2 / 3;
                _graphics.PreferredBackBufferHeight = 1080 * 2 / 3;
            }
            _graphics.ApplyChanges();
            IsMouseVisible = !_graphics.IsFullScreen;

            var gen = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[_randomNumbers.Length * 2];
            gen.GetBytes(bytes);
            for (int i = 0; i < _randomNumbers.Length; i++)
            {
                int x = 0;
                x |= bytes[i * 2];
                x |= (int)bytes[i * 2 + 1] << 8;
                _randomNumbers[i] = x % 2048;
            }

            _obstacles = new List<Obstacle>();

            ResetGame();
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Resources.Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Resources.Pixel.SetData(new[] { Color.White });
            Resources.Player = Content.Load<Texture2D>("sprit");
            Resources.PlayerForward = Content.Load<Texture2D>("fwdspritesheet");
            Resources.PlayerBackward = Content.Load<Texture2D>("revspritesheet");
            Resources.Background = Content.Load<Texture2D>("background");
            Resources.Paralax1 = Content.Load<Texture2D>("parralax1");
            Resources.Paralax2 = Content.Load<Texture2D>("parrrlalal_2");

            Resources.Static = new Texture2D(GraphicsDevice, 1, 4096);
            var colors = new Color[Resources.Static.Height];
            var rnd = new Random(0);
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = (rnd.Next(2) == 0) ? Color.White : Color.Black;
            }
            Resources.Static.SetData(colors);

            Resources.Circle = new Texture2D(GraphicsDevice, 200, 200);
            colors = new Color[Resources.Circle.Width * Resources.Circle.Height];
            int r = Resources.Circle.Width / 2 - 2;
            for (int x = 0; x < Resources.Circle.Width; x++)
            {
                for (int y = 0; y < Resources.Circle.Height; y++)
                {
                    double dx = (Resources.Circle.Width - 1) / 2.0 - x;
                    double dy = (Resources.Circle.Height - 1) / 2.0 - y;
                    double dist = Math.Abs(r - Math.Sqrt(dx * dx + dy * dy));
                    double k;
                    if (dist >= 2)
                    {
                        k = 0;
                    }
                    else
                    {
                        dist = 6 - 6 * dist;
                        k = Math.Exp(dist) / (Math.Exp(dist) + 1);
                    }
                    colors[x + y * Resources.Circle.Width] = Color.White * (float)k;
                }
            }
            Resources.Circle.SetData(colors);

            _renderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080);
            _waveRenderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080);
            
            Resources.Obstacles.Add(new Obstacle(false, true, new Vector(0, 810), Content.Load<Texture2D>("obstacle1"), new Polygon(
                new Vector(23, 810 - 0),
                new Vector(33, 810 - 462),
                new Vector(161, 810 - 391),
                new Vector(155, 810 - 122),
                new Vector(180, 810 - 0)
            )));
            Resources.Obstacles.Add(new Obstacle(false, false, new Vector(0, 65), Content.Load<Texture2D>("obstacle2"), new Polygon(
                new Vector(422, 65 - 335),
                new Vector(390, 65 - 14),
                new Vector(333, 65 - 26),
                new Vector(249, 65 - 178),
                new Vector(102, 65 - 211),
                new Vector(22, 65 - 335)
            )));
            Resources.Obstacles.Add(new Obstacle(false, false, new Vector(0, 258), Content.Load<Texture2D>("obstacle3"), new Polygon(
                new Vector(355, 258 - 528),
                new Vector(438, 258 - 19),
                new Vector(160, 258 - 213),
                new Vector(103, 258 - 224),
                new Vector(98, 258 - 285),
                new Vector(10, 258 - 528)
            )));
            Resources.Obstacles.Add(new Obstacle(false, true, new Vector(0, 810), Content.Load<Texture2D>("obstacle4"), new Polygon(
                new Vector(12, 810 - 0),
                new Vector(20, 810 - 267),
                new Vector(206, 810 - 326),
                new Vector(517, 810 - 160),
                new Vector(534, 810 - 241),
                new Vector(580, 810 - 273),
                new Vector(950, 810 - 194),
                new Vector(1034, 810 - 0)
            )));
            Resources.Obstacles.Add(new Obstacle(false, true, new Vector(0, 810), Content.Load<Texture2D>("obstacle5"), new Polygon(
                new Vector(5, 810 - 0),
                new Vector(78, 810 - 178),
                new Vector(300, 810 - 163),
                new Vector(304, 810 - 233),
                new Vector(580, 810 - 390),
                new Vector(600, 810 - 447),
                new Vector(765, 810 - 380),
                new Vector(793, 810 - 274),
                new Vector(893, 810 - 134),
                new Vector(978, 810 - 94),
                new Vector(1006, 810 - 0)
            )));
            Resources.Obstacles.Add(new Obstacle(true, true, new Vector(0, 810), Content.Load<Texture2D>("obstacle6"), new Polygon(
                new Vector(26, 810 - 0),
                new Vector(162, 810 - 52),
                new Vector(279, 810 - 34),
                new Vector(325, 810 - 63),
                new Vector(372, 810 - 61),
                new Vector(446, 810 - 37),
                new Vector(526, 810 - 47),
                new Vector(642, 810 - 27),
                new Vector(696, 810 - 49),
                new Vector(781, 810 - 23),
                new Vector(927, 810 - 33),
                new Vector(974, 810 - 52),
                new Vector(1164, 810 - 14),
                new Vector(1220, 810 - 15),
                new Vector(1240, 810 - 0)
            )));
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_nextTopSlot < _player.Position.X + 2300)
            {
                PlaceTop(false);
            }

            if (_deathTimer > 0)
            {
                _deathTimer++;
                if (_deathTimer > 90)
                {
                    ResetGame();
                }
            }
            
            _tick++;

            double dt = 1 / 90.0;
            _waves.Update(dt, _player.Position.X);
            _player.Update(dt, _waves);

            for (int i = _obstacles.Count - 1; i >= 0; i--)
            {
                if (_obstacles[i].Right < _player.Position.X - 500)
                {
                    _obstacles[i] = _obstacles[_obstacles.Count - 1];
                    _obstacles.RemoveAt(_obstacles.Count - 1);
                }
            }

            if (_deathTimer == 0)
            {
                var center = _player.CollisionCenter;
                var radius = _player.CollisionRadius;
                for (int i = 0; i < _obstacles.Count; i++)
                {
                    if (_obstacles[i].IntersectsCircle(center, radius))
                    {
                        _deathTimer = 1;
                        _player.Kill();
                        break;
                    }
                }
            }

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Vector camera = new Vector(_player.Position.X - 250, 810);

            GraphicsDevice.SetRenderTarget(_waveRenderTarget);
            GraphicsDevice.Clear(new Color(0, 0, 0, 0));
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            DrawWaves(camera);
            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.SetRenderTarget(_renderTarget);
            _spriteBatch.Begin();
            DrawGame(camera);
            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            _spriteBatch.Draw(_waveRenderTarget, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White * 0.3f);
            if (DebugInfo)
            {
                Microphone.DrawDebugInfo(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGame(Vector camera)
        {
            DrawBackground(camera);
            for (int i = 0; i < _obstacles.Count; i++)
            {
                _obstacles[i].Draw(_spriteBatch, camera);
            }
            _player.Draw(_spriteBatch, camera);
        }

        private void DrawWaves(Vector camera)
        {
            for (int i = 0; i < 1920 / 2; i++)
            {
                double wave = _waves.GetHeight(camera.X + i * 2);
                double freeSpace = camera.Y - wave;
                int h = (int)Math.Round(freeSpace);
                int startH = _randomNumbers[(_tick * 1920 + i) % _randomNumbers.Length];
                int texH = (1080 - h + 1) / 2;

                _spriteBatch.Draw(Resources.Static, new Rectangle(i * 2, h, 2, texH * 2), new Rectangle(0, startH, 1, texH), Color.White);
            }
        }

        private void DrawBackground(Vector camera)
        {
            _spriteBatch.Draw(Resources.Background, Vector2.Zero, Color.White);

            const double ParalaxSpeed1 = 0.15;
            double pt = -ParalaxSpeed1 * camera.X;
            pt = (pt % (1920 * 4) + (1920 * 4)) % (1920 * 4);
            int x = (int)Math.Round(pt);
            _spriteBatch.Draw(Resources.Paralax1, new Rectangle(x, 0, 1920 * 4, 1080), Color.White);
            _spriteBatch.Draw(Resources.Paralax1, new Rectangle(x - 1920 * 4, 0, 1920 * 4, 1080), Color.White);

            const double ParalaxSpeed2 = 0.4;
            pt = -ParalaxSpeed2 * camera.X;
            pt = (pt % (1920 * 2) + (1920 * 2)) % (1920 * 2);
            x = (int)Math.Round(pt);
            int a = 200;
            var color = new Color(a, a, a);
            _spriteBatch.Draw(Resources.Paralax2, new Rectangle(x, 0, 1920 * 2, 1080), color);
            _spriteBatch.Draw(Resources.Paralax2, new Rectangle(x - 1920 * 2, 0, 1920 * 2, 1080), color);
        }

        private void PlaceTop(bool placeLong)
        {
            if (_forceTopLong)
            {
                _forceTopLong = false;
                placeLong = true;
            }
            int good = 0;
            for (int i = 0; i < Resources.Obstacles.Count; i++)
            {
                if (Resources.Obstacles[i].IsTop && (!placeLong || Resources.Obstacles[i].IsLong))
                {
                    good++;
                }
            }

            if (good == 0)
            {
                throw new Exception("no good obstacles");
            }

            good = _rnd.Next(good);
            for (int i = 0; i < Resources.Obstacles.Count; i++)
            {
                if (Resources.Obstacles[i].IsTop && (!placeLong || Resources.Obstacles[i].IsLong))
                {
                    if (good-- == 0)
                    {
                        var ob = Resources.Obstacles[i];
                        if (!ob.IsLong)
                        {
                            _forceTopLong = true;
                        }
                        Vector offset = new Vector(_nextTopSlot - ob.Left - (1 + _rnd.NextDouble()) * 100, 0);
                        ob = new Obstacle(ob, offset);
                        _nextTopSlot = ob.Right;
                        _obstacles.Add(ob);
                        return;
                    }
                }
            }
        }

        private void ResetGame()
        {
            _obstacles.Clear();

            _nextTopSlot = -500;
            _nextBotSlot = -500;
            _forceTopLong = false;
            _forceBotLong = false;
            _deathTimer = 0;
            for (int i = 0; i < 3; i++) PlaceTop(true);

            _player = new Player(new Vector(0, 0));

            _waves = new ExpandedWave(new ScaledWave(new CompositeWave(
                new ScaledWave(new MovingWave(new SinWave(200), 100), 0.1),
                new ScaledWave(new MovingWave(new SinWave(100), -25), 0.05),
                new MovingWave(new ScaledWave(new SimulatedWave(), 0.1), 100),
                new SmoothedWave(new MovingWave(new Microwave(), 350), 30)
            ), 100), 2);
        }
    }
}
