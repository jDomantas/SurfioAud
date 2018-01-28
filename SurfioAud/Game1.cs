using Microsoft.Xna.Framework;  
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurfioAud.Geometry;
using SurfioAud.Waves;
using System;
using System.Collections.Generic;
using System.Globalization;

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

            _graphics.IsFullScreen = true;
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
            Resources.Player[0] = Content.Load<Texture2D>("idle");
            Resources.Player[1] = Content.Load<Texture2D>("idleb1");
            Resources.Player[2] = Content.Load<Texture2D>("idleb2");
            Resources.Player[3] = Content.Load<Texture2D>("idleb3");
            Resources.PlayerForward[0] = Content.Load<Texture2D>("fwd");
            Resources.PlayerForward[1] = Content.Load<Texture2D>("fwdb1");
            Resources.PlayerForward[2] = Content.Load<Texture2D>("fwdb2");
            Resources.PlayerForward[3] = Content.Load<Texture2D>("fwdb3");
            Resources.PlayerBackward[0] = Content.Load<Texture2D>("rev");
            Resources.PlayerBackward[1] = Content.Load<Texture2D>("revb1");
            Resources.PlayerBackward[2] = Content.Load<Texture2D>("revb2");
            Resources.PlayerBackward[3] = Content.Load<Texture2D>("revb3");
            Resources.PlayerDeath = Content.Load<Texture2D>("deth");
            Resources.Background = Content.Load<Texture2D>("background");
            Resources.Paralax1 = Content.Load<Texture2D>("parralax1");
            Resources.Paralax2 = Content.Load<Texture2D>("parrrlalal_2");
            Resources.Font = Content.Load<SpriteFont>("Fyodor");

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
            Resources.Obstacles.Add(new Obstacle(false, false, new Vector(0, 258 - 100), Content.Load<Texture2D>("obstacle3"), new Polygon(
                new Vector(355, 258 - 528 - 100),
                new Vector(438, 258 - 19 - 100),
                new Vector(160, 258 - 213 - 100),
                new Vector(103, 258 - 224 - 100),
                new Vector(98, 258 - 285 - 100),
                new Vector(10, 258 - 528 - 100)
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
                new Vector(2, 810 - 0),
                new Vector(17, 810 - 32),
                new Vector(239, 810 - 62),
                new Vector(468, 810 - 33),
                new Vector(633, 810 - 25),
                new Vector(668, 810 - 0)
            )));
            Resources.Obstacles.Add(new Obstacle(true, true, new Vector(0, 810), Content.Load<Texture2D>("obstacle7"), new Polygon(
                new Vector(13, 810 - 0),
                new Vector(22, 810 - 26),
                new Vector(268, 810 - 33),
                new Vector(658, 810 - 95),
                new Vector(673, 810 - 138),
                new Vector(780, 810 - 42),
                new Vector(905, 810 - 77),
                new Vector(1000, 810 - 6),
                new Vector(1050, 810 - 30),
                new Vector(1065, 810 - 0)
            )));
            Resources.Obstacles.Add(new Obstacle(true, true, new Vector(0, 810), Content.Load<Texture2D>("obstacle8"), new Polygon(
                new Vector(6, 810 - 0),
                new Vector(18, 810 - 30),
                new Vector(438, 810 - 52),
                new Vector(584, 810 - 76),
                new Vector(741, 810 - 64),
                new Vector(1000, 810 - 19),
                new Vector(1214, 810 - 8),
                new Vector(1224, 810 - 0)
            )));
            Resources.Obstacles.Add(new Obstacle(true, true, new Vector(0, 810), Content.Load<Texture2D>("obstacle9"), new Polygon(
                new Vector(8, 810 - 0),
                new Vector(99, 810 - 46),
                new Vector(128, 810 - 81),
                new Vector(202, 810 - 56),
                new Vector(538, 810 - 59),
                new Vector(609, 810 - 40),
                new Vector(654, 810 - 45),
                new Vector(807, 810 - 0)
            )));
            Resources.Obstacles.Add(new Obstacle(true, false, new Vector(0, -201), Content.Load<Texture2D>("obstacle6"), new Polygon(
                new Vector(668, -270 + 0),
                new Vector(633, -270 + 25),
                new Vector(468, -270 + 33),
                new Vector(239, -270 + 62),
                new Vector(17, -270 + 32),
                new Vector(2, -270 + 0)
            )).VFlipped());
            Resources.Obstacles.Add(new Obstacle(true, false, new Vector(0, -124), Content.Load<Texture2D>("obstacle7"), new Polygon(
                new Vector(1065, -270 + 0),
                new Vector(1050, -270 + 30),
                new Vector(1000, -270 + 6),
                new Vector(905, -270 + 77),
                new Vector(780, -270 + 42),
                new Vector(673, -270 + 138),
                new Vector(658, -270 + 95),
                new Vector(268, -270 + 33),
                new Vector(22, -270 + 26),
                new Vector(13, -270 + 0)
            )).VFlipped());
            Resources.Obstacles.Add(new Obstacle(true, false, new Vector(0, -174), Content.Load<Texture2D>("obstacle8"), new Polygon(
                new Vector(1224, -270 + 0),
                new Vector(1214, -270 + 8),
                new Vector(1000, -270 + 19),
                new Vector(741, -270 + 64),
                new Vector(584, -270 + 76),
                new Vector(438, -270 + 52),
                new Vector(18, -270 + 30),
                new Vector(6, -270 + 0)
            )).VFlipped());
            Resources.Obstacles.Add(new Obstacle(true, false, new Vector(0, -170), Content.Load<Texture2D>("obstacle9"), new Polygon(
                new Vector(807, -270 + 0),
                new Vector(654, -270 + 45),
                new Vector(609, -270 + 40),
                new Vector(538, -270 + 59),
                new Vector(202, -270 + 56),
                new Vector(128, -270 + 81),
                new Vector(99, -270 + 46),
                new Vector(8, -270 + 0)
            )).VFlipped());
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            while (_nextTopSlot < _player.Position.X + 2300)
            {
                PlaceTop(false);
            }
            while (_nextBotSlot < _player.Position.X + 2300)
            {
                PlaceBot(false);
            }

            if (_deathTimer > 0)
            {
                _deathTimer++;
                if (_deathTimer > 100)
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
            _spriteBatch.End();
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.DrawString(Resources.Font, Math.Round(_player.Position.X / 50, 0).ToString(CultureInfo.InvariantCulture), new Vector2(10, 10), Color.White);
            if (DebugInfo)
            {
                Microphone.DrawDebugInfo(_spriteBatch);
            }
            if (_deathTimer > 60)
            {
                for (int i = 0; i < 1920 / 2; i++)
                {
                    int startH = _randomNumbers[(_tick * 1920 + i) % _randomNumbers.Length];
                    _spriteBatch.Draw(Resources.Static, new Rectangle(i * 2, 0, 2, 1080), new Rectangle(0, startH, 1, 1080 / 2), Color.White);
                }
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
            double prevHeight = 0;
            for (int i = 0; i < 1920 / 2; i++)
            {
                double wave = _waves.GetHeight(camera.X + i * 2);
                double freeSpace = camera.Y - wave;
                int h = (int)Math.Round(freeSpace);
                int startH = _randomNumbers[(_tick * 1920 + i) % _randomNumbers.Length];
                int texH = (1080 - h + 1) / 2;

                _spriteBatch.Draw(Resources.Static, new Rectangle(i * 2, h, 2, texH * 2), new Rectangle(0, startH, 1, texH), Color.White);

                if (i > 0)
                {
                    double from = Math.Max(wave, prevHeight), to = Math.Min(wave, prevHeight);
                    freeSpace = camera.Y - from;
                    h = (int)Math.Round(freeSpace);
                    int colH = (int)Math.Round(from - to);
                    _spriteBatch.Draw(Resources.Pixel, new Rectangle(i * 2 - 1, h - 1, 4, colH + 4), Color.Black);
                    prevHeight = wave;
                }
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
                        Vector offset = new Vector(_nextTopSlot - ob.Left - (1 + _rnd.NextDouble()) * 50, 0);
                        ob = new Obstacle(ob, offset);
                        _nextTopSlot = ob.Right;
                        _obstacles.Add(ob);
                        return;
                    }
                }
            }
        }

        private void PlaceBot(bool placeLong)
        {
            if (_forceBotLong)
            {
                _forceBotLong = false;
                placeLong = true;
            }
            int good = 0;
            for (int i = 0; i < Resources.Obstacles.Count; i++)
            {
                if (!Resources.Obstacles[i].IsTop && (!placeLong || Resources.Obstacles[i].IsLong))
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
                if (!Resources.Obstacles[i].IsTop && (!placeLong || Resources.Obstacles[i].IsLong))
                {
                    if (good-- == 0)
                    {
                        var ob = Resources.Obstacles[i];
                        if (!ob.IsLong)
                        {
                            _forceBotLong = true;
                        }
                        Vector offset = new Vector(_nextBotSlot - ob.Left - (1 + _rnd.NextDouble()) * 50, 0);
                        ob = new Obstacle(ob, offset);
                        _nextBotSlot = ob.Right;
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
            for (int i = 0; i < 3; i++)
            {
                PlaceTop(true);
                PlaceBot(true);
            }

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
