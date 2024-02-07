using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Dynamic;

namespace nancet_minigolf
{
    public class Game1 : Game
    {
        private Ball ball;
        private Rectangle[] course;
        private Wall[] walls;
        private static int strokes;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private class Ball : DrawableGameComponent
        {
            private static Texture2D _ballSprite;
            public Rectangle hitbox;
            public Vector2 position;
            public Vector2 velocity;
            public float strength;

            public Ball(Game game) : base(game) { }

            protected override void LoadContent()
            {
                if (_ballSprite == null) _ballSprite = Game.Content.Load<Texture2D>("golfBall");
                position = new Vector2(120, 25);
                velocity = Vector2.Zero;
                hitbox = new Rectangle((int)position.X, (int)position.Y, 25, 25);
                base.LoadContent();
            }

            public override void Update(GameTime gameTime)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space)) Reset();

                if (Mouse.GetState().LeftButton == ButtonState.Pressed && 
                    GraphicsDevice.Viewport.Bounds.Contains(Mouse.GetState().Position) &&
                    velocity == Vector2.Zero)
                {
                    velocity.X = (Mouse.GetState().X - position.X)/2;
                    velocity.Y = (Mouse.GetState().Y - position.Y)/2;
                    strokes++;
                }

                if (velocity != Vector2.Zero) strength -= 1;
                else strength = Mouse.GetState().ScrollWheelValue>12000 ? 100 : Mouse.GetState().ScrollWheelValue / 120;

                position += velocity * (float)(gameTime.ElapsedGameTime.TotalSeconds);
                
                if (strength < 1)
                {
                    strength = 0;
                    if (Math.Abs(velocity.X) > 1 || Math.Abs(velocity.Y) > 1) velocity *= 0.98f;
                    else velocity = Vector2.Zero;
                }

                hitbox.X = (int)position.X;
                hitbox.Y = (int)position.Y;

                base.Update(gameTime);
            }

            public override void Draw(GameTime gameTime)
            {
                ((SpriteBatch)(Game.Services.GetService(typeof(SpriteBatch)))).Draw(_ballSprite, hitbox, Color.White);
                base.Draw(gameTime);
            }

            public void Reset()
            {
                position = new Vector2(120, 25);
                velocity = Vector2.Zero;
                strokes += 2;
            }
        }

        private class Wall : DrawableGameComponent
        {
            private static Texture2D _wallSprite;
            public Rectangle hitbox;
            public bool isVertical;

            public Wall(Game game, int x, int y, int width, int height) : base(game)
            {
                hitbox = new Rectangle(x, y, width, height);
                isVertical = height > width;
            }

            protected override void LoadContent()
            {
                if (_wallSprite == null) _wallSprite = Game.Content.Load<Texture2D>("wall");
                base.LoadContent();
            }

            public override void Draw(GameTime gameTime)
            {
                ((SpriteBatch)(Game.Services.GetService(typeof(SpriteBatch)))).Draw(_wallSprite, hitbox, Color.Black);
                base.Draw(gameTime);
            }

            public bool isTouching(Rectangle obj)
            {
                return hitbox.Intersects(obj);
            }
        }

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
            course = new Rectangle[8];
            walls = new Wall[10];

            course[0] = new Rectangle(  5,   5, 250, 500);
            course[1] = new Rectangle(405, 205, 100, 300);
            course[2] = new Rectangle(255, 205, 150, 150);
            course[3] = new Rectangle(255, 405, 100, 100);

            course[4] = new Rectangle(265, 355, 140, 50);
            course[5] = new Rectangle(355, 405, 50, 100);
            course[6] = new Rectangle(100, 185, 64, 64);

            course[7] = new Rectangle(440, 440, 30, 30);

            walls[0] = new Wall(this,   5,   5, 10, 500);
            walls[1] = new Wall(this,   5,   5, 250, 10);
            walls[2] = new Wall(this, 255,   5, 10, 210);
            walls[3] = new Wall(this,   5, 505, 350, 10);
            walls[4] = new Wall(this, 265, 205, 240, 10);
            walls[5] = new Wall(this, 505, 205, 10, 300);
            walls[6] = new Wall(this, 405, 505, 110, 10);
            walls[7] = new Wall(this, 255, 355, 10,  50);
            walls[8] = new Wall(this, 405, 355, 10,  50);
            walls[9] = new Wall(this, 255, 405, 100, 10);

            Components.Add(ball);
            for (int i = 0; i < 10 && walls[i] != null; i++)
            {
                Components.Add(walls[i]);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), _spriteBatch);

            /*
             * TODO:
             * add sounds for strokes, bounces, hole
             * add ending effect when player wins
             */
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            for(int i=0; i<10 && walls[i]!=null; i++)
            {
                if (walls[i].isTouching(ball.hitbox))
                {
                    if (walls[i].isVertical) ball.velocity.X *= -.9f;
                    else ball.velocity.Y *= -.9f;
                }
            }

            if (course[2].Intersects(ball.hitbox))
            {
                ball.velocity.X += ball.velocity.X>200 ? 0 : 5;
                ball.velocity.Y += 1 * (ball.velocity.Y > 0 ? 1 : -1);
            }

            if (course[3].Intersects(ball.hitbox))
            {
                ball.velocity.X -= .25f;
                ball.velocity.Y += .1f * (ball.velocity.Y > 0 ? 1 : -1);
            }

            if (course[6].Intersects(ball.hitbox)) ball.velocity = 0.8f * Vector2.Reflect(ball.velocity, Vector2.Normalize(ball.velocity));

            if (course[4].Intersects(ball.hitbox)) ball.Reset();
            if (course[5].Intersects(ball.hitbox) && ball.velocity.X <= 10) ball.Reset();

            if (course[7].Intersects(ball.hitbox) && (ball.velocity.X < 50 && ball.velocity.Y < 50)) ball.Reset();

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

            _spriteBatch.Draw(Content.Load<Texture2D>("blank"), course[4], Color.CornflowerBlue);
            _spriteBatch.Draw(Content.Load<Texture2D>("blank"), course[5], Color.CornflowerBlue);
            _spriteBatch.Draw(Content.Load<Texture2D>("rock1"), course[6], Color.White);

            _spriteBatch.Draw(Content.Load<Texture2D>("golfBall"), course[7], Color.Black);

            _spriteBatch.DrawString(Content.Load<SpriteFont>("File"), "Par: 2", new Vector2(400,10), Color.Black);
            _spriteBatch.DrawString(Content.Load<SpriteFont>("File"), "Strokes: " + strokes, new Vector2(400, 40), Color.Black);
            _spriteBatch.DrawString(Content.Load<SpriteFont>("File"), "Strength: " + ball.strength.ToString(), new Vector2(400, 70), Color.Black);

            base.Draw(gameTime);

            _spriteBatch.End();
        }
    }
}
