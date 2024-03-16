using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BEPUphysics;
using System.Threading;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;
//using System.Numerics;

namespace nancet_spacerace
{
    public class Game1 : Game
    {
        internal class Level
        {
            public static List<Level> levels = new List<Level>();
            public Ring[] rings;
            public int[] starScores = new int[3];
            public Level(Ring[] rings, int[] starTimeScoresLowToHigh)
            {
                this.rings = rings;
                starScores = starTimeScoresLowToHigh;
                levels.Add(this);
            }
            public static void LoadLevel(int number)
            {
                if (number >= levels.Count) return;
                Ring.courseSequence.Clear();
                foreach (Ring ring in levels[number].rings)
                {
                    Ring.courseSequence.Add(ring);
                    ring.isVisible = true;
                }
            }
            public static void NextLevel()
            {
                if (levels.Count == 0) return;
                levels.RemoveAt(0);
                if (levels.Count > 0) LoadLevel(0);
            }
        }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Space space = new Space();
        private Ship ship;
        private Skybox skybox;
        private SpriteFont timer, dialogue;
        private Texture2D star, pixel_white;

        public enum GameState { GAMEOVER, PLAYING, PAUSED, STOPPED };
        public static GameState _state;

        public static float[] time = new float[2];

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            _state = GameState.PLAYING;
            Services.AddService<Space>(space);

            ship = new Ship(this);
            Services.AddService<Ship>(ship);

            new Level(new Ring[] {
                    new Ring(this, new Vector3(0, 0, -5), Vector3.Forward),
                    new Ring(this, new Vector3(2, 0, -10), new Vector3(1, 0, -1)),
                    new Ring(this, new Vector3(5, -1, -17), new Vector3(1, -.2f, -1)),
                    new Ring(this, new Vector3(8, -3, -23), new Vector3(.5f, 0, -1)),
                    new Ring(this, new Vector3(13, -3, -27), new Vector3(1, 0, -.2f)),
                    new Ring(this, new Vector3(19, -3, -27), new Vector3(1, 0, .2f)),
                    new Ring(this, new Vector3(22, -3, -23), new Vector3(.5f, 0, 1)),
                    new Ring(this, new Vector3(25, -1, -17), new Vector3(1, .2f, 1)),
                    new Ring(this, new Vector3(28, 0, -10), new Vector3(1, 0, 1)),
                    new Ring(this, new Vector3(30, 0, -5), Vector3.Forward)
                },
                new int[] {13,16,24}
            );
            new Level(new Ring[]
                {
                    new Ring(this, new Vector3(0,0,-5), Vector3.Forward),
                    new Ring(this, new Vector3(0,0,-50), Vector3.Forward)
                },
                new int[] {7,8,9}
            );
            new Level(new Ring[]
                {
                    new Ring(this, new Vector3(0,0,-5), Vector3.Forward),
                    new Ring(this, new Vector3(0,0,-15), Vector3.Forward),
                    new Ring(this, new Vector3(3,0,-20), new Vector3(1.3f,0,-1)),
                    new Ring(this, new Vector3(10,0,-20), Vector3.Right),
                    new Ring(this, new Vector3(15,1,-20), new Vector3(1,1,0)),
                    new Ring(this, new Vector3(15,6,-20), new Vector3(-1,1,0)),
                    new Ring(this, new Vector3(10,7,-20), Vector3.Left),
                    new Ring(this, new Vector3(3,7,-22), new Vector3(-1.3f,0,-1)),
                    new Ring(this, new Vector3(0,7,-27), Vector3.Forward),
                    new Ring(this, new Vector3(3,7,-30), new Vector3(1.3f,0,-1)),
                    new Ring(this, new Vector3(10,7,-30), Vector3.Right),
                    new Ring(this, new Vector3(20,7,-25), new Vector3(-.3f,0,-1)),
                    new Ring(this, new Vector3(24,7,-20), new Vector3(.3f,0,-1)),
                    new Ring(this, new Vector3(20,7,-17), new Vector3(-.3f,0,-1)),
                    new Ring(this, new Vector3(24,7,-14), new Vector3(.3f,0,-1)),
                    new Ring(this, new Vector3(10,7,-10), Vector3.Left),
                    new Ring(this, new Vector3(-5,7,-10), Vector3.Left),
                },
                new int[] {17,20,27}
            );

            Level.LoadLevel(0);
            
            skybox = new Skybox(this);

            IsMouseVisible = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            timer = Content.Load<SpriteFont>("2D\\Timer");
            dialogue = Content.Load<SpriteFont>("2D\\Dialogue");
            star = Content.Load<Texture2D>("2D\\Star");
            pixel_white = Content.Load<Texture2D>("2D\\Pixel_White");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (_state!=GameState.PLAYING && (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)))
            {
                Exit();
            }

            if (Keyboard.GetState().CapsLock) _state = GameState.PAUSED;
            else _state = GameState.PLAYING;
            space.Update();

            if(Ring.courseSequence.Count <= 0) _state = GameState.GAMEOVER;

            if(_state == GameState.PLAYING)
            {
                IsMouseVisible = false;
                if(Ring.courseSequence.Count > 0) time[1] += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (time[1] >= 60) { time[0]++; time[1] -= 60; }
            } else IsMouseVisible = true;

            if (_state == GameState.GAMEOVER)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    Level.NextLevel();
                    ship.Reset();
                    time = new float[2] { 0, 0 };
                    _state = GameState.PLAYING;
                }
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            
            base.Draw(gameTime);

            //Shoutout to this link: https://gamedev.stackexchange.com/questions/31616/spritebatch-begin-making-my-model-not-render-correctly
            //For helping me write text to the screen
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            _spriteBatch.Begin();

            if(_state == GameState.GAMEOVER)
            {
                _spriteBatch.DrawString(timer, ((int)time[0]).ToString() + ":" + Math.Round(time[1], 2).ToString(), new Vector2(GraphicsDevice.Viewport.Width / 2 - 50, GraphicsDevice.Viewport.Height / 2 - 100), Color.Aqua);
                _spriteBatch.DrawString(dialogue, "Missed: " + Ring.missedRings.ToString(), new Vector2(GraphicsDevice.Viewport.Width / 2 - 50, GraphicsDevice.Viewport.Height / 2 + 100), Color.Aqua);
                if(Level.levels.Count > 1) _spriteBatch.DrawString(dialogue, "Press [ENTER] for next level", new Vector2(GraphicsDevice.Viewport.Width / 2 - 150, GraphicsDevice.Viewport.Height / 2 + 125), Color.Aqua);
                _spriteBatch.Draw(star, new Rectangle(GraphicsDevice.Viewport.Width / 2 - 25, GraphicsDevice.Viewport.Height / 2 - 25, 50, 50), (time[0] * 60 + time[1] > Level.levels[0].starScores[0] ? Color.Black : Color.White));
                _spriteBatch.Draw(star, new Rectangle(GraphicsDevice.Viewport.Width / 2 - 100, GraphicsDevice.Viewport.Height / 2, 50, 50), (time[0] * 60 + time[1] > Level.levels[0].starScores[1] ? Color.Black : Color.White));
                _spriteBatch.Draw(star, new Rectangle(GraphicsDevice.Viewport.Width / 2 + 50, GraphicsDevice.Viewport.Height / 2, 50, 50), (time[0] * 60 + time[1] > Level.levels[0].starScores[2] ? Color.Black : Color.White));
            } else
            {
                _spriteBatch.DrawString(timer, ((int)time[0]).ToString() + ":" + Math.Round(time[1], 2).ToString(), new Vector2(GraphicsDevice.Viewport.Width - 150, 5), Color.Aqua);
                _spriteBatch.DrawString(dialogue, "Missed: " + Ring.missedRings.ToString(), new Vector2(GraphicsDevice.Viewport.Width - 150, 50), Color.Aqua);
                int target;
                if (time[0] * 60 + time[1] > Level.levels[0].starScores[0])
                {
                    if (time[0] * 60 + time[1] > Level.levels[0].starScores[1]) target = Level.levels[0].starScores[2];
                    else target = Level.levels[0].starScores[1];
                } else target = Level.levels[0].starScores[0];
                _spriteBatch.DrawString(dialogue, "Target: " + (target/60).ToString()+":"+(target%60).ToString(), new Vector2(GraphicsDevice.Viewport.Width - 150, 75), (time[0] * 60 + time[1] > Level.levels[0].starScores[2] ? Color.Red : Color.Aqua));
                _spriteBatch.Draw(pixel_white, new Rectangle(5, GraphicsDevice.Viewport.Height - 50, (int)(100 * ship.Boost), 20), Color.Aqua);
            }
            
            _spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
