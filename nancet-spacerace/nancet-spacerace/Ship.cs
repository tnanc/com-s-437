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
        private BEPUphysics.Entities.Prefabs.Cone shipPhysics;
        public Vector3 position, rotation;

        public Ship(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public Ship(Game game, Vector3 position) : this(game)
        {
            this.position = position;
            rotation = Vector3.Zero;
            shipPhysics = new BEPUphysics.Entities.Prefabs.Cone(ConversionHelper.MathConverter.Convert(position), 5, 5, 1);
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

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.Services.GetService<Camera>().RenderModel(model);

            base.Draw(gameTime);
        }
    }
}