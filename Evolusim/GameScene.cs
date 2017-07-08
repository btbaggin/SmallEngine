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
        Brush _backgroundBrush;
        public override void Begin()
        {
            base.Begin();

            _backgroundBrush = Game.Graphics.CreateBrush(System.Drawing.Color.Red);
            InputManager.Listen(Keys.Escape);
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            base.Draw(pSystem);
            pSystem.DrawRect(new System.Drawing.RectangleF(0, 0, Game.Form.Width, Game.Form.Height), _backgroundBrush);
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            if(InputManager.IsPressed(Keys.Escape))
            {
                SceneManager.BeginScene(new MenuScene(true), SceneLoadMode.Additive);
            }
        }

        public override void End()
        {
            base.End();

            _backgroundBrush.Dispose();
            InputManager.StopListening(Keys.Escape);
        }
    }
}
