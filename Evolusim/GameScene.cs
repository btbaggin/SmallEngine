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

        public override void Begin()
        {
            base.Begin();

            InputManager.Listen(Keys.Escape);
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            base.Draw(pSystem);
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            if(InputManager.IsPressed(Keys.Escape))
            {
                //TODO load menu
            }
        }

        public override void End()
        {
            base.End();

            InputManager.StopListening(Keys.Escape);
        }
    }
}
