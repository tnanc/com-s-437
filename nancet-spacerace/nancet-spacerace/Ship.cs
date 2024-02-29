using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BEPUphysics;
using System.Threading;
using static nancet_spacerace.Game1;

namespace nancet_spacerace
{
    public class Ship : DrawableGameComponent
    {
        private Model model;
        private Texture2D texture;
        private BEPUphysics.Entities.Prefabs.Sphere shipPhysics;
        public Vector3 position, rotation; //(x,y,z) and (yaw,pitch,roll)

        public Ship(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public Ship(Game game, Vector3 position) : this(game)
        {
            this.position = position;
            rotation = Vector3.Zero;
            shipPhysics = new BEPUphysics.Entities.Prefabs.Sphere(MathConverter.Convert(position), 1);
            shipPhysics.Mass = 1;
            shipPhysics.BecomeKinematic();
            Game.Services.GetService<Space>().Add(shipPhysics);
        }

        public override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: Load your game content here
            texture = Game.Content.Load<Texture2D>("Spaceship\\ShipTexture");
            model = Game.Content.Load<Model>("Spaceship\\Ship");
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            if (Keyboard.GetState().IsKeyDown(Keys.W)) shipPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateRotationX(MathHelper.TwoPi / -360f)) * shipPhysics.WorldTransform;
            else if (Keyboard.GetState().IsKeyDown(Keys.S)) shipPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateRotationX(MathHelper.TwoPi / 360f)) * shipPhysics.WorldTransform;

            if (Keyboard.GetState().IsKeyDown(Keys.A)) shipPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateRotationZ(MathHelper.TwoPi / 360f)) * shipPhysics.WorldTransform;
            else if (Keyboard.GetState().IsKeyDown(Keys.D)) shipPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateRotationZ(MathHelper.TwoPi / -360f)) * shipPhysics.WorldTransform;

            if (Keyboard.GetState().IsKeyDown(Keys.Q)) shipPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateRotationY(MathHelper.TwoPi / 360f)) * shipPhysics.WorldTransform;
            else if (Keyboard.GetState().IsKeyDown(Keys.E)) shipPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateRotationY(MathHelper.TwoPi / -360f)) * shipPhysics.WorldTransform;


            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) shipPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateTranslation(new Vector3(0f, .1f, 0f))) * shipPhysics.WorldTransform;
                else shipPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateTranslation(new Vector3(0f,-.1f,0f))) * shipPhysics.WorldTransform;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Camera camera = Game.Services.GetService<Camera>();
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = MathConverter.Convert(shipPhysics.WorldTransform);
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}