using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BEPUphysics;
using System.Threading;

namespace nancet_spacerace
{
    public class Ship : DrawableGameComponent
    {
        private Model model;
        private Texture2D texture;
        private BEPUphysics.Entities.Entity shipPhysics;
        public Vector3 position;

        public Ship(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: Load your game content here
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}