﻿using Microsoft.Xna.Framework;
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

//            _waves = new SimulatedWave();
            _waves = new CompositeWave(
                new ScaledWave(new MovingWave(new SinWave(200), 200), 0.35),
                new ScaledWave(new MovingWave(new SinWave(160), 170), 0.15),
                new ScaledWave(new MovingWave(new SinWave(58), -25), 0.02)
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

            _waves.Update(1 / 60.0);

//            if (tick++ % 180 == 0)
//                _waves.Splash();
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            const double baseHeight = 450;
            const double scale = 30;

            _spriteBatch.Begin();

            for (int i = 0; i < 800; i++)
            {
                int start = (int)Math.Round(baseHeight - scale * _waves.GetHeight(i));
                _spriteBatch.Draw(_pixel, new Rectangle(i * 2, start, 2, 900 - start), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
