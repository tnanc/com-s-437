using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BEPUphysics;
using System.Threading;
using static nancet_spacerace.Game1;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework.Media;

namespace nancet_spacerace
{
    public class Ship : DrawableGameComponent
    {
        private ObjectSpace ship;
        private float Velocity { get; set; } = 5f;
        //can change radians higher/lower for sensitivity?
        private float Radians { get; set; } = .03f;
        private MouseState prevMouse;
        private Random random;
        private float isShaking = 0;
        private Song thrust;
        public float Boost { get; private set; } = 2.5f;
        private static void Events_InitialCollisionDetected(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            //Debug.WriteLine("Collision Detected");
        }
        public Ship(Game game) : base(game)
        {
            ship = new ObjectSpace(Game.GraphicsDevice);
            ship.World = Matrix.CreateWorld(Vector3.Zero,Vector3.Forward,Vector3.Up);
            ship.PhysicsObject = new BEPUphysics.Entities.Prefabs.Sphere(MathConverter.Convert(ship.Position), 1);
            ship.PhysicsObject.CollisionInformation.CollisionRules.Personal = BEPUphysics.CollisionRuleManagement.CollisionRule.NoSolver;
            ship.PhysicsObject.CollisionInformation.Events.InitialCollisionDetected += Events_InitialCollisionDetected;
            ship.PhysicsObject.BecomeKinematic();
            Game.Components.Add(this);
            Game.Services.GetService<Space>().Add(ship.PhysicsObject);
            random = new Random();
        }

        public override void Initialize()
        {
            // TODO: Add your initialization logic here
            prevMouse = Mouse.GetState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: Load your game content here
            ship.Texture = Game.Content.Load<Texture2D>("Spaceship\\CockpitTexture");
            ship.Model = Game.Content.Load<Model>("Spaceship\\Cockpit");
            thrust = Game.Content.Load<Song>("Sounds\\Rocket Thrusters");
            MediaPlayer.Play(thrust);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keys = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (Game1._state == Game1.GameState.PLAYING)
            {

                //Moves camera based on mouse position
                Vector2 diff = mouse.Position.ToVector2() - prevMouse.Position.ToVector2();
                Debug.WriteLine("Y: "+ship.Up.Y+(ship.Up.Y < 0 ? " < 0" : " >= 0"));
                if (diff.X != 0f) ship.PhysicsObject.WorldTransform = MathConverter.Convert(Matrix.CreateRotationY(diff.X * -1f * (float)gameTime.ElapsedGameTime.TotalSeconds)) * ship.PhysicsObject.WorldTransform;
                if (diff.Y != 0f) ship.PhysicsObject.WorldTransform = MathConverter.Convert(Matrix.CreateRotationX(diff.Y * -1f * (float)gameTime.ElapsedGameTime.TotalSeconds)) * ship.PhysicsObject.WorldTransform;
                //Resets mouse position if outside of bounds
                if (mouse.X < 0 || mouse.X > Game.GraphicsDevice.Viewport.Width) Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, mouse.Y);
                if (mouse.Y < 0 || mouse.Y > Game.GraphicsDevice.Viewport.Height) Mouse.SetPosition(mouse.X, Game.GraphicsDevice.Viewport.Height / 2);

                //Moves camera based on key input
                if (keys.IsKeyDown(Keys.W)) ship.PhysicsObject.WorldTransform = MathConverter.Convert(Matrix.CreateRotationX(Radians)) * ship.PhysicsObject.WorldTransform;
                else if (keys.IsKeyDown(Keys.S)) ship.PhysicsObject.WorldTransform = MathConverter.Convert(Matrix.CreateRotationX(Radians * -1f)) * ship.PhysicsObject.WorldTransform;

                if (keys.IsKeyDown(Keys.A)) ship.PhysicsObject.WorldTransform = MathConverter.Convert(Matrix.CreateRotationY(Radians)) * ship.PhysicsObject.WorldTransform;
                else if (keys.IsKeyDown(Keys.D)) ship.PhysicsObject.WorldTransform = MathConverter.Convert(Matrix.CreateRotationY(Radians * -1f)) * ship.PhysicsObject.WorldTransform;

                if (keys.IsKeyDown(Keys.Q)) ship.PhysicsObject.WorldTransform = MathConverter.Convert(Matrix.CreateRotationZ(Radians)) * ship.PhysicsObject.WorldTransform;
                else if (keys.IsKeyDown(Keys.E)) ship.PhysicsObject.WorldTransform = MathConverter.Convert(Matrix.CreateRotationZ(Radians * -1f)) * ship.PhysicsObject.WorldTransform;

                if(Boost < 2.5f && !(keys.IsKeyDown(Keys.LeftControl) || keys.IsKeyDown(Keys.RightControl))) Boost += (float)gameTime.ElapsedGameTime.TotalSeconds*2;

                //Thrust and Booster
                if (keys.IsKeyDown(Keys.Space))
                {
                    MediaPlayer.Volume = (isShaking > .2f ? 2 : .5f);
                    if (keys.IsKeyDown(Keys.LeftControl) || keys.IsKeyDown(Keys.RightControl))
                    {
                        if (isShaking < .5)
                        {
                            ship.PhysicsObject.WorldTransform = MathConverter.Convert(Matrix.CreateRotationX((float)(random.NextDouble() - 0.5) / 15f)) * ship.PhysicsObject.WorldTransform;
                            isShaking += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        Boost -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else { isShaking = 0;  }
                    //Left Shift to go backwards, Left Control for added speed
                    if (keys.IsKeyDown(Keys.LeftShift)) ship.PhysicsObject.WorldTransform *= MathConverter.Convert(Matrix.CreateTranslation((ship.Forward * Velocity * (isShaking > .2f ? 3 : 1) * -0.8f) * (float)gameTime.ElapsedGameTime.TotalSeconds));
                    else ship.PhysicsObject.WorldTransform *= MathConverter.Convert(Matrix.CreateTranslation((ship.Forward * Velocity * (isShaking>0 && Boost>0 ? isShaking*5 : 1)) * (float)gameTime.ElapsedGameTime.TotalSeconds));
                } else MediaPlayer.Volume = 0;

                ship.World = MathConverter.Convert(ship.PhysicsObject.WorldTransform);
                ObjectSpace.RenderView(ship);
            } else MediaPlayer.Volume = 0; 

            prevMouse = mouse;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            RasterizerState r = new RasterizerState();
            r.CullMode = CullMode.None;
            Game.GraphicsDevice.RasterizerState = r;
            foreach (ModelMesh mesh in ship.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Tab))
                    {
                        
                        //ObjectSpace.View *= (float)random.NextDouble();
                        //ObjectSpace.Projection *= (float)random.NextDouble();
                    }
                    if (_state == GameState.PAUSED) effect.Alpha = .5f;
                    else if (_state == GameState.PLAYING) effect.Alpha = 1;
                    effect.World = MathConverter.Convert(ship.PhysicsObject.WorldTransform);
                    effect.View = ObjectSpace.View;
                    effect.Projection = ObjectSpace.Projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
            RasterizerState r1 = new RasterizerState();
            r1.CullMode = CullMode.CullCounterClockwiseFace;
            Game.GraphicsDevice.RasterizerState = r1;

            base.Draw(gameTime);
        }

        public void Reset()
        {
            ship.World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            ship.PhysicsObject.WorldTransform = MathConverter.Convert(ship.World);
            ObjectSpace.RenderView(ship);
        }
    }
}