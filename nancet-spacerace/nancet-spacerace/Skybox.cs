using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BEPUphysics;
using System.Threading;
using System.Collections.Generic;
using static nancet_spacerace.Game1;

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
            texture = Game.Content.Load<Texture2D>("Skybox\\Stars");
            model = Game.Content.Load<Model>("Skybox\\Model");
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
                    if (_state == GameState.PAUSED) effect.Alpha = .5f;
                    else if (_state == GameState.PLAYING) effect.Alpha = 1;
                    effect.World = Matrix.CreateScale(10000f);
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