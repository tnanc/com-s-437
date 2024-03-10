using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BEPUphysics;
using System.Threading;
//using System.Numerics;

namespace nancet_spacerace
{
    public class Camera : ObjectSpace
    {
        private Game game;
        private float Velocity { get; set; } = 30f;
        private float Radians { get; set; } = 60f;
        
        private void RenderWorldView()
        {
            World = Matrix.CreateWorld(Position, Forward, Up);
            ObjectSpace.RenderView(this);
        }

        public void RotateAround(Vector3 axis, bool positive, GameTime gameTime)
        {
            Forward = Vector3.TransformNormal(Forward, Matrix.CreateFromAxisAngle(
                axis, MathHelper.ToRadians(Radians * (float)gameTime.ElapsedGameTime.TotalSeconds * (positive ? 1 : -1))));
            RenderWorldView();
        }
        public void Roll(bool clockwise, GameTime gameTime)
        {
            Vector3 tmp = Position;
            World *= Matrix.CreateFromAxisAngle(Forward, MathHelper.ToRadians(Radians * (float)gameTime.ElapsedGameTime.TotalSeconds * (clockwise ? 1 : -1)));
            Position = tmp;
            RenderWorldView();
        }
        public void Move(bool forward, GameTime gameTime)
        {
            Position += (Forward * Velocity) * 
                (float)gameTime.ElapsedGameTime.TotalSeconds * (forward ? 1 : -1);
            RenderWorldView();
        }

        public Camera(Game game) : base(game.GraphicsDevice,null,null)
        {
            ObjectSpace.RenderView(this);
        }

        public void Update(GameTime gameTime)
        {
            if(Keyboard.GetState().IsKeyDown(Keys.W)) RotateAround(World.Right, true, gameTime);
            else if (Keyboard.GetState().IsKeyDown(Keys.S)) RotateAround(World.Right, false, gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.A)) RotateAround(World.Up, true, gameTime);
            else if (Keyboard.GetState().IsKeyDown(Keys.D)) RotateAround(World.Up, false, gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.Q)) Roll(false, gameTime);
            else if (Keyboard.GetState().IsKeyDown(Keys.E)) Roll(true, gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.Space)) Move(!(Keyboard.GetState().IsKeyDown(Keys.LeftShift)), gameTime);
        }
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Space space = new Space();
        private Camera camera;
        public static Ship ship;
        private Skybox skybox;

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

            //camera = new Camera(this);
            //Services.AddService<Camera>(camera);

            //camera = new Basic3dExampleCamera(GraphicsDevice, Window);
            camera = new Camera(this);

            camera.Position = new Vector3(2, 10, 52);
            camera.Forward = Vector3.Forward;
            Services.AddService<Camera>(camera);

            ship = new Ship(this, new Vector3(10,10,10));
            new Ring(this, new Vector3(0, 0, -10));
            skybox = new Skybox(this);

            

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

            camera.Update(gameTime);
            space.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Indigo);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
