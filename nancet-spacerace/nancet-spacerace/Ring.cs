using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BEPUphysics;
using System.Threading;
using System.Collections.Generic;

namespace nancet_spacerace
{
    public class Ring : DrawableGameComponent
    {
        public static List<Ring> courseSequence = new List<Ring>();
        private int index;
        private Model model;
        private Texture2D[] textures = new Texture2D[2];
        public bool isActive, isVisible;
        public Vector3 position, rotation;
        public BEPUphysics.Entities.Prefabs.Sphere ringPhysics, centerPhysics;

        public Ring(Game game) : base(game)
        {
            game.Components.Add(this);
            courseSequence.Add(this);
            index = courseSequence.Count - 1;
            isActive = (index==0);
            isVisible = true;
        }

        public Ring(Game game, Vector3 position) : this(game)
        {
            this.position = position;
            this.rotation = Vector3.Zero;
            ringPhysics = new BEPUphysics.Entities.Prefabs.Sphere(MathConverter.Convert(position), 1);
            Game.Services.GetService<Space>().Add(ringPhysics);
            //initialize physics objects here
        }

        public void setActive()
        {
            isActive = true;
            //set texture to be green, might just happen in the Draw method
            for (int i=1; index-i >= 0 && courseSequence[index - i].isVisible; i++) {
                courseSequence[index - i].miss();
            }
        }

        public void pass()
        {
            isActive = false;
            isVisible = false;
            courseSequence[index+1].setActive();
        }

        public void miss()
        {
            isActive = false;
            isVisible = false;
            Game1.time += 10f;
        }

        public override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: Load your game content here
            textures[0] = Game.Content.Load<Texture2D>("Ring\\ringPowerTexture");
            textures[1] = Game.Content.Load<Texture2D>("Ring\\ringMetalTexture");
            model = Game.Content.Load<Model>("Ring\\Ring2");
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) ringPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateTranslation(new Vector3(0f, .1f, 0f))) * ringPhysics.WorldTransform;
            else if (Keyboard.GetState().IsKeyDown(Keys.Down)) ringPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateTranslation(new Vector3(0f, -.1f, 0f))) * ringPhysics.WorldTransform;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            /*
            Camera camera = Game.Services.GetService<Camera>();
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = camera.World;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
            */

            base.Draw(gameTime);
        }
    }
}