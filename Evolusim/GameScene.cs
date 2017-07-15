﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        Toolbar _toolbar;

        protected override void Begin()
        {
            base.Begin();

            _terrain = new Terrain(513, 513);
            _toolbar = new Toolbar();
            UIManager.Register(_toolbar);
            InputManager.Listen(Keys.T);
            InputManager.Listen(Keys.Escape);
            InputManager.Listen(Keys.Up);
            InputManager.Listen(Keys.Down);
            InputManager.Listen(Keys.Left);
            InputManager.Listen(Keys.Right);

            InputManager.Listen(Mouse.Left);

            //for(int i = 0; i < 100; i++)
            //{
                Organism.Create();
            //}
        }

        protected override void Draw(IGraphicsSystem pSystem)
        {
            base.Draw(pSystem);
            _terrain.Draw(pSystem);
        }

        protected override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            if(InputManager.KeyPressed(Keys.Escape))
            {
                SceneManager.BeginScene(new MenuScene(true), SceneLoadMode.Additive);
            }

            if(InputManager.KeyDown(Mouse.Left))
            {
                _terrain.SetTypeAt(_toolbar.SelectedType, Evolusim.ActiveCamera.ToWorldSpace(InputManager.MousePosition));
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
