using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BEPUphysics;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using static nancet_spacerace.Game1;

namespace nancet_spacerace
{
    public class Ring : DrawableGameComponent
    {
        public static List<Ring> courseSequence = new List<Ring>();
        public static int missedRings = 0;
        public bool isVisible;
        private ObjectSpace ring;
        private void Events_InitialCollisionDetected(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            if(isVisible) this.pass();
        }

        public Ring(Game game) : base(game)
        {
            game.Components.Add(this);
            courseSequence.Add(this);
            isVisible = false;
        }

        public Ring(Game game, Vector3 position, Vector3 forward) : this(game)
        {
            ring = new ObjectSpace(Game.GraphicsDevice);
            ring.Position = position;
            ring.Forward = forward;
            ring.PhysicsObject = new BEPUphysics.Entities.Prefabs.Sphere(MathConverter.Convert(ring.Position), 1);
            ring.PhysicsObject.Mass = 0;
            ring.PhysicsObject.BecomeKinematic();
            ring.PhysicsObject.CollisionInformation.Events.InitialCollisionDetected += Events_InitialCollisionDetected;
            ring.PhysicsObject.WorldTransform = MathConverter.Convert(ring.World);
            Game.Services.GetService<Space>().Add(ring.PhysicsObject);
        }

        private void miss()
        {
            Game1.time[1] += 10f;
            missedRings++;
        }

        private void pass()
        {
            int i = 0;
            while (courseSequence[0] != this)
            {
                courseSequence[0].isVisible = false;
                courseSequence.RemoveAt(0);
                if(i > 0) miss();
                if (courseSequence.Count <= 0) break;
                i++;
            }
            if (courseSequence.Count <= 0) return;
            courseSequence[0].isVisible = false;
            courseSequence.RemoveAt(0);
            if (i > 0) miss();
        }

        public override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: Load your game content here
            ring.Texture = Game.Content.Load<Texture2D>("Ring\\Ring_Default");
            ring.Model = Game.Content.Load<Model>("Ring\\Ring");
            
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            /*
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) ringPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateTranslation(new Vector3(0f, .1f, 0f))) * ringPhysics.WorldTransform;
            else if (Keyboard.GetState().IsKeyDown(Keys.Down)) ringPhysics.WorldTransform = MathConverter.Convert(Matrix.CreateTranslation(new Vector3(0f, -.1f, 0f))) * ringPhysics.WorldTransform;
            */
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            if(!isVisible) return;

            foreach (ModelMesh mesh in ring.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    if (_state == GameState.PAUSED) effect.Alpha = .25f;
                    else if (_state == GameState.PLAYING)
                    {
                        if (courseSequence[0] == this)
                        {
                            effect.Texture = Game.Content.Load<Texture2D>("Ring\\Ring_Active");
                            effect.Alpha = 1f;
                        }
                        else
                        {
                            effect.Texture = ring.Texture;
                            effect.Alpha = 0.5f;
                        }
                    }
                    effect.World = ring.World;
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