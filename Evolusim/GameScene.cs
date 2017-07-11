using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.Input;

namespace Evolusim
{
    class GameScene : Scene
    {
        Terrain _terrain;
        Toolbar _toolbar;

        public override void Begin()
        {
            base.Begin();

            _terrain = new Terrain(513, 513);
            _toolbar = new Toolbar();
            InputManager.Listen(Keys.T);
            InputManager.Listen(Keys.Escape);
            InputManager.Listen(Mouse.Left);
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            base.Draw(pSystem);
            _terrain.Draw(pSystem);
            _toolbar.Draw(pSystem);
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            _toolbar.Update(pDeltaTime);

            if(InputManager.KeyPressed(Keys.Escape))
            {
                SceneManager.BeginScene(new MenuScene(true), SceneLoadMode.Additive);
            }

            if(InputManager.KeyDown(Mouse.Left))
            {
                _terrain.SetTypeAt(_toolbar.SelectedType, Evolusim.MainCamera.ToWorldSpace(InputManager.MousePosition));
            }

            if(InputManager.KeyPressed(Keys.T))
            {
                _toolbar.Toggle();
            }
        }

        public override void End()
        {
            base.End();

            InputManager.StopListening(Keys.T);
            InputManager.StopListening(Keys.Escape);
            InputManager.StopListening(Mouse.Left);
        }
    }
}
