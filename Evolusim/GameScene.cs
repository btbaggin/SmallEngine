using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.Input;
using Evolusim.UI;
using SmallEngine.UI;

namespace Evolusim
{
    class GameScene : Scene
    {
        Terrain _terrain;
        VegetationMap _vegetationMap;
        Minimap _minimap;
        Toolbar _toolbar;

        protected override void Begin()
        {
            base.Begin();

            _terrain = new Terrain();
            _vegetationMap = new VegetationMap();
            _minimap = new Minimap(_terrain, 200, 256);
            _toolbar = new Toolbar();
            UIManager.Register(_toolbar);
            UIManager.Register(_minimap);
            InputManager.Listen(Keys.T);
            InputManager.Listen(Keys.Escape);
            InputManager.Listen(Keys.Up);
            InputManager.Listen(Keys.Down);
            InputManager.Listen(Keys.Left);
            InputManager.Listen(Keys.Right);

            InputManager.Listen(Mouse.Left);

            for(int i = 0; i < 100; i++)
            {
                Organism.Create();
            }
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            base.Draw(pSystem);
            _terrain.Draw(pSystem);
            _vegetationMap.Draw(pSystem);
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            _vegetationMap.Update(pDeltaTime);

            if(InputManager.KeyPressed(Keys.Escape))
            {
                _toolbar.Hide();
                SceneManager.BeginScene(new MenuScene(true), SceneLoadMode.Additive);
            }

            if(InputManager.KeyDown(Mouse.Left))
            {
                Terrain.SetTypeAt(_toolbar.SelectedType, Evolusim.ActiveCamera.ToWorldSpace(InputManager.MousePosition));
            }

            if(InputManager.KeyPressed(Keys.T))
            {
                _toolbar.Toggle();
            }
        }

        protected override void End()
        {
            base.End();

            UIManager.Unregister(_toolbar);
            UIManager.Unregister(_minimap);
            _minimap.Dispose();
            _toolbar.Dispose();

            InputManager.StopListening(Keys.T);
            InputManager.StopListening(Keys.Escape);
            InputManager.StopListening(Keys.Up);
            InputManager.StopListening(Keys.Down);
            InputManager.StopListening(Keys.Left);
            InputManager.StopListening(Keys.Right);

            InputManager.StopListening(Mouse.Left);
        }
    }
}
