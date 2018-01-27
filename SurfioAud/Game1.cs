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
       
        private IWave _waves;
        private Player _player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
 
        protected override void Initialize()
        {
            base.Initialize();

            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();
            IsMouseVisible = true;

            _waves = new ExpandedWave(new ScaledWave(new CompositeWave(
                new ScaledWave(new MovingWave(new SinWave(200), 100), 0.1),
                new ScaledWave(new MovingWave(new SinWave(100), -25), 0.05),
                new SmoothedWave(new MovingWave(new Microwave(), 200), 30)
            ), 100), 2);

            _player = new Player(new Vector(0, 0));
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Resources.Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Resources.Pixel.SetData(new[] { Color.White });
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _waves.Update(1 / 60.0, _player.Position.X);
            _player.Update(1 / 60.0, _waves);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin();

            Vector camera = new Vector(_player.Position.X - 150, _graphics.PreferredBackBufferHeight * 0.5);
            RenderBackground(camera);
            RenderWaves(camera);
            _player.Draw(_spriteBatch, camera);


            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void RenderWaves(Vector camera)
        {
            for (int i = 0; i < 800; i++)
            {
                double wave = _waves.GetHeight(camera.X + i * 2);
                double freeSpace = camera.Y - wave;
                int h = (int)Math.Round(freeSpace);
                _spriteBatch.Draw(Resources.Pixel, new Rectangle(i * 2, h, 2, _graphics.PreferredBackBufferHeight - h), Color.White);
            }
        }

        private void RenderBackground(Vector camera)
        {
            double first = Math.Floor(camera.X / 300) - 1;
            for (int i = 0; i < 10; i++)
            {
                Vector pos = new Vector((first + i) * 300, 400);
                pos -= camera;
                int x = (int)Math.Round(pos.X);
                int y = (int)Math.Round(pos.Y);
                _spriteBatch.Draw(Resources.Pixel, new Rectangle(x - 10, -(y - 10), 20, 20), Color.Black);
            }
        }
    }
}
