using Microsoft.Xna.Framework;  
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurfioAud.Waves;
using System;

namespace SurfioAud
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly int[] _randomNumbers;

        private IWave _waves;
        private Player _player;
        private RenderTarget2D _renderTarget;
        private RenderTarget2D _waveRenderTarget;
        private int _tick;
        
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
            IsMouseVisible = true;

            _waves = new ExpandedWave(new ScaledWave(new CompositeWave(
                new ScaledWave(new MovingWave(new SinWave(200), 100), 0.1),
                new ScaledWave(new MovingWave(new SinWave(100), -25), 0.05),
                new MovingWave(new ScaledWave(new SimulatedWave(), 0.1), 100),
                new SmoothedWave(new MovingWave(new Microwave(), 350), 30)
            ), 100), 2);

            _player = new Player(new Vector(0, 0));

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

            _renderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080);
            _waveRenderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080);
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _tick++;

            double dt = 1 / 90.0;
            _waves.Update(dt, _player.Position.X);
            _player.Update(dt, _waves);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Vector camera = new Vector(_player.Position.X - 250, 1080 * 0.75);

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
            Microphone.DrawDebugInfo(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGame(Vector camera)
        {
            DrawBackground(camera);
            // DrawWaves(camera);
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

            double first = Math.Floor(camera.X / 300) - 1;
            for (int i = 0; i < 10; i++)
            {
                Vector pos = new Vector((first + i) * 300, 400);
                pos -= camera;
                x = (int)Math.Round(pos.X);
                int y = (int)Math.Round(pos.Y);
                _spriteBatch.Draw(Resources.Pixel, new Rectangle(x - 10, -(y - 10), 20, 20), Color.Black);
            }
        }
    }
}
