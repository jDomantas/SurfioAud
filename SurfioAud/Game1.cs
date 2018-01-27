using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurfioAud.Waves;
using System;

namespace SurfioAud
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Wavess _waves;
        Texture2D _pixel;
        int tick;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
 
        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();
            IsMouseVisible = true;

            _waves = new Wavess(); new CompositeWave(
                new ScaledWave(new SinWave(200, 200), 0.35),
                new ScaledWave(new SinWave(160, 170), 0.15),
                new ScaledWave(new SinWave(58, -25), 0.02)
            );
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _waves.Update(1 / 60.0);

            if (tick++ % 180 == 0)
                _waves.Splash();
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            const double BaseHeight = 450;
            const double Scale = 30;

            spriteBatch.Begin();

            for (int i = 0; i < 800; i++)
            {
                int start = (int)Math.Round(BaseHeight - Scale * _waves.GetHeight(i));
                spriteBatch.Draw(_pixel, new Rectangle(i * 2, start, 2, 900 - start), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
