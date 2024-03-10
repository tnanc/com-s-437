using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BEPUphysics;
using System.Threading;
using System.Collections.Generic;

namespace nancet_spacerace
{
    public class Skybox : DrawableGameComponent
    {
        private Model model;
        private Texture2D texture;

        public Skybox(Game game) : base(game) 
        {
            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Skybox\\SpaceTexture");
            model = Game.Content.Load<Model>("Skybox\\Skybox");
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            RasterizerState r = new RasterizerState();
            r.CullMode = CullMode.None;
            Game.GraphicsDevice.RasterizerState = r;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateScale(1000f);
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
    }
}