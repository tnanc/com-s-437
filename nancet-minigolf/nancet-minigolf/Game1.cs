using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace nancet_minigolf
{
    public class Game1 : Game
    {

        private class Ball : DrawableGameComponent
        {
            private static Texture2D _ballSprite;
            public Vector2 position;
            public Vector2 velocity;
            public float strength;

            public Ball(Game game) : base(game) { }

            protected override void LoadContent()
            {
                if (_ballSprite == null) _ballSprite = Game.Content.Load<Texture2D>("golfBall");
                position = new Vector2(120, 25);
                velocity = Vector2.Zero;
                base.LoadContent();
            }

            public override void Update(GameTime gameTime)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Space)) position = new Vector2(120, 25);

                if (Mouse.GetState().LeftButton == ButtonState.Pressed && GraphicsDevice.Viewport.Bounds.Contains(Mouse.GetState().Position))
                {
                    velocity.X = (Mouse.GetState().X - position.X)/2;
                    velocity.Y = (Mouse.GetState().Y - position.Y)/2;
                }

                if (velocity != Vector2.Zero) strength -= 1;
                else strength = Mouse.GetState().ScrollWheelValue/40;

                position += velocity * (float)(gameTime.ElapsedGameTime.TotalSeconds);
                
                if (strength < 1)
                {
                    strength = 0;
                    if (Math.Abs(velocity.X) > 1 || Math.Abs(velocity.Y) > 1) velocity *= 0.98f;
                    else velocity = Vector2.Zero;
                }

                base.Update(gameTime);
            }

            public override void Draw(GameTime gameTime)
            {
                ((SpriteBatch)(Game.Services.GetService(typeof(SpriteBatch)))).Draw(_ballSprite, new Rectangle((int)position.X,(int)position.Y,25,25), Color.White);
                base.Draw(gameTime);
            }
        }

        private Ball ball;
        private Rectangle[] course;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 550;
            _graphics.PreferredBackBufferHeight = 550;
            _graphics.ApplyChanges();

            ball = new Ball(this);
            course = new Rectangle[4];

            course[0] = new Rectangle(5, 5, 250, 500);
            course[1] = new Rectangle(405, 205, 100, 300);
            course[2] = new Rectangle(255, 205, 150, 150);
            course[3] = new Rectangle(255, 405, 100, 100);

            Components.Add(ball);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), _spriteBatch);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            _spriteBatch.Draw(Content.Load<Texture2D>("courseColor"), course[0], Color.White);
            _spriteBatch.Draw(Content.Load<Texture2D>("downColor"), course[1], Color.White);
            _spriteBatch.Draw(Content.Load<Texture2D>("slopeDownColor"), course[2], Color.White);
            _spriteBatch.Draw(Content.Load<Texture2D>("slopeUpColor"), course[3], Color.White);

            _spriteBatch.Draw(Content.Load<Texture2D>("golfBall"), new Rectangle(440,440,30,30), Color.Black);

            _spriteBatch.DrawString(Content.Load<SpriteFont>("File"), ball.strength.ToString() , new Vector2(10,10), Color.Black);

            base.Draw(gameTime);

            _spriteBatch.End();
        }
    }
}
