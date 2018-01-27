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
        private Texture2D _pixel;
        int tick;
        Microphone _mic;

        float[] values;
        int nextPos;

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
            
            _waves = new CompositeWave(
                new ScaledWave(new MovingWave(new SinWave(200), 200), 0.35),
                new ScaledWave(new MovingWave(new SinWave(160), 170), 0.15),
                new ScaledWave(new MovingWave(new SinWave(58), -25), 0.02)
            );

            _mic = new Microphone();

            values = new float[180];
            nextPos = 0;
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            values[nextPos++] = _mic.GetTickValue();
            nextPos %= 180;
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin();
            
            for (int i = 0; i < 180; i++)
            {
                int pos = (nextPos + i) % 180;
                int h = (int)Math.Round(values[pos] / 1000 * 450);
                _spriteBatch.Draw(_pixel, new Rectangle(i * 8 + 40, 450 - h, 8, h), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
