using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BEPUphysics;
using System.Threading;
//using System.Numerics;

namespace nancet_spacerace
{
    public class Camera
    {
        public Matrix world { get; private set; }
        public Matrix view { get; private set; }
        public Matrix projection { get; private set; }
        public Vector3 Position, Target, Up;

        public Camera()
        {
            Position = new Vector3(0, 0, 10);
            Target = Vector3.Zero;
            Up = Vector3.UnitY;

            world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            view = Matrix.CreateLookAt(Position, Target, Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);
        }

        public void RenderModel(Model model)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
            
        }
    }

    public class Game1 : Game
    {
        

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Space space = new Space();
        private Camera camera { get; set; }
        private Ship ship;

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

            camera = new Camera();
            Services.AddService<Camera>(camera);

            ship = new Ship(this, Vector3.Zero);

            IsMouseVisible = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        private static float[] previousMousePos = { 0, 0 };

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
