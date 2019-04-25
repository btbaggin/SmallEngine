using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.Input;
using Evolusim.UI;
using SmallEngine.UI;
using SmallEngine.Messages;

using Evolusim.Terrain;

namespace Evolusim
{
    class GameScene : Scene
    {
        TerrainMap _terrain;
        Minimap _minimap;
        InspectionBar _toolbar;

        protected override void Begin()
        {
            base.Begin();

            InitializeLevel();

            UIManager.Register(_toolbar);
            UIManager.Register(_minimap);

            InputManager.Listen(Keys.Escape);
            InputManager.Listen(Keys.Up);
            InputManager.Listen(Keys.Down);
            InputManager.Listen(Keys.Left);
            InputManager.Listen(Keys.Right);

            InputManager.Listen(Mouse.Left);
            InputManager.Listen(Mouse.Right);
        }

        //TODO change terrain to rendercomponent
        //public override void Draw(IGraphicsAdapter pSystem)
        //{
        //    _terrain.Draw(pSystem);
        //    base.Draw(pSystem);
        //}

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            if (InputManager.KeyPressed(Keys.Escape))
            {
                _toolbar.Hide();
                BeginScene(new MenuScene(true), SceneLoadMode.Additive);
            }

            if (InputManager.KeyPressed(Mouse.Left) && !InputManager.HasFocus())
            {
                Game.Messages.SendMessage(new GameMessage("ToolbarClose", null));
            }

            else if (InputManager.KeyPressed(Mouse.Right) && Organism.SelectedOrganism != null)
            {
                Organism.SelectedOrganism.MoveTo(Game.ActiveCamera.ToWorldSpace(InputManager.MousePosition));
            }
        }

        protected override void End()
        {
            base.End();

            UIManager.Unregister(_toolbar);
            UIManager.Unregister(_minimap);
            _minimap.Dispose();
            _toolbar.Dispose();

            InputManager.StopListening(Keys.Escape);
            InputManager.StopListening(Keys.Up);
            InputManager.StopListening(Keys.Down);
            InputManager.StopListening(Keys.Left);
            InputManager.StopListening(Keys.Right);

            InputManager.StopListening(Mouse.Left);
            InputManager.StopListening(Mouse.Right);
        }

        private void InitializeLevel()
        {
            //Create terrain
            _terrain = new TerrainMap();
            _minimap = new Minimap(_terrain, 200, 256);
            _toolbar = new InspectionBar();

            Vegetation.Populate();

            for (int i = 0; i < 5; i++)
            {
                var x = Generator.Random.Next(0, 100);
                var y = Generator.Random.Next(0, 100);
                EnemySpawner.Create(x, y);
            }


            for (int i = 0; i < 100; i++)
            {
                Organism.Create();
            }
        }
    }
}
