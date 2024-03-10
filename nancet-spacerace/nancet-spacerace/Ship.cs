using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BEPUphysics;
using System.Threading;
using static nancet_spacerace.Game1;
using System.Collections.Generic;

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
            rotation = new Vector3(0f,-1.57f,1.57f);
            shipPhysics = new BEPUphysics.Entities.Prefabs.Sphere(MathConverter.Convert(position), 1);
            shipPhysics.Mass = 1;
            shipPhysics.BecomeKinematic();
            Game.Services.GetService<Space>().Add(shipPhysics);
        }

        private void RotateModel(Vector3 angle)
        {
            rotation += angle;
        }

        private void MoveModel(Vector3 speed)
        {
            //position += speed;
            shipPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateTranslation(speed)) * shipPhysics.WorldTransform;
            position = MathConverter.Convert(shipPhysics.Position);
        }

        public override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: Load your game content here
            texture = Game.Content.Load<Texture2D>("Spaceship\\Spaceship_Texture");
            model = Game.Content.Load<Model>("Spaceship\\Spaceships_8");
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            if (Keyboard.GetState().IsKeyDown(Keys.W)) RotateModel(new Vector3(0f, 0.1f, 0f));
            else if (Keyboard.GetState().IsKeyDown(Keys.S)) RotateModel(new Vector3(0f, -0.1f, 0f));
            
            if (Keyboard.GetState().IsKeyDown(Keys.A)) RotateModel(new Vector3(0.1f, 0f, 0f));
            else if (Keyboard.GetState().IsKeyDown(Keys.D)) RotateModel(new Vector3(-0.1f, 0f, 0f));

            if (Keyboard.GetState().IsKeyDown(Keys.Q)) RotateModel(new Vector3(0f, 0f, 0.1f));
            else if (Keyboard.GetState().IsKeyDown(Keys.E)) RotateModel(new Vector3(0f, 0f, -0.1f));

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) MoveModel(new Vector3(0f, 0f, 1f)); //shipPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateTranslation(new Vector3(0f, .1f, 0f))) * shipPhysics.WorldTransform;
                else MoveModel(new Vector3(0f,0f,-1f));
            }
            
            shipPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z) * Matrix.CreateTranslation(position));
            //Game.Services.GetService<Camera>().Render((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Camera camera = Game.Services.GetService<Camera>();
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.Identity; //MathConverter.Convert(shipPhysics.WorldTransform)*Matrix.CreateScale(.05f);
                    effect.View = ObjectSpace.View;
                    effect.Projection = ObjectSpace.Projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}