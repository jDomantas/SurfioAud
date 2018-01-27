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
                new ScaledWave(new MovingWave(new SinWave(53), 60), 0.03),
                new ScaledWave(new MovingWave(new SinWave(20), -25), 0.02),
                new SmoothedWave(new MovingWave(new Microwave(), 200), 30)
            );
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

            _waves.Update(1 / 60.0, 0);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin();
            
            for (int i = 0; i < 800; i++)
            {
                int h = (int)Math.Round(_waves.GetHeight(i) * 50 + 450);
                _spriteBatch.Draw(_pixel, new Rectangle(i * 2, 900 - h, 2, h), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
