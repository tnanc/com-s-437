using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BEPUphysics;
using System.Threading;

namespace nancet_spacerace
{
    public class Game1 : Game
    {
        protected internal class Camera
        {
            public Vector3 position { get; set; }
            public Vector3 direction { get; set; }
            public Vector3 zAxis { get; set; }

            public Camera()
            {
                position = Vector3.Zero;
                direction = Vector3.Forward;
                zAxis = Vector3.Up;
            }
        }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Space space = new Space();
        private Camera camera { get; set; }

        protected enum GameState { GAMEOVER, PLAYING, PAUSED , STOPPED };
        protected GameState _state;

        public static float time = 0f;

        //private Timer _timer;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Services.AddService<Space>(space);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

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

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
