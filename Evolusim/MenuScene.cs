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
    class MenuScene : Scene
    {
        private List<string> mItems;
        private int mCurrentIndex;

        public MenuScene() : base()
        {
            mItems = new List<string>(3)
            {
                "New",
                "Continue",
                "Quit"
            };
            mCurrentIndex = 0;
        }

        public override void Begin()
        {
            base.Begin();
            InputManager.Listen(Keys.Up);
            InputManager.Listen(Keys.Down);
            InputManager.Listen(Keys.Enter);
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            base.Draw(pSystem);

            //TODO
            pSystem.DefineColor(System.Drawing.Color.Aqua);
            pSystem.DefineColor(System.Drawing.Color.Black);
            pSystem.DefineColor(System.Drawing.Color.White);

            pSystem.DrawRect(new System.Drawing.RectangleF(0, 0, Game.Form.Width, Game.Form.Height), System.Drawing.Color.Aqua);

            var mid = Game.Form.Width / 2;

            System.Drawing.Point p = new System.Drawing.Point(mid, 50);
            for(int i = 0; i < mItems.Count; i++)
            {
                if(mCurrentIndex == i)
                {
                    pSystem.DrawText(mItems[i], p, System.Drawing.Color.White);
                }
                else
                {
                    pSystem.DrawText(mItems[i], p, System.Drawing.Color.Black);
                }
                p.Y += 10;
            }
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            if(InputManager.IsPressed(Keys.Up))
            {
                if (mCurrentIndex == 0) mCurrentIndex = mItems.Count - 1;
                else mCurrentIndex--;
            }
            else if(InputManager.IsPressed(Keys.Down))
            {
                if (mCurrentIndex == mItems.Count - 1) mCurrentIndex = 0;
                else mCurrentIndex++;
            }

            if(InputManager.IsPressed(Keys.Enter))
            {
                End();
            }
        }

        public override void End()
        {
            base.End();

            switch(mCurrentIndex)
            {
                case 0:
                case 1:
                    var scene = new GameScene();
                    //TODO
                    break;
                case 2:
                    Game.IsPlaying = false;
                    break;
            }

            InputManager.StopListening(Keys.Up);
            InputManager.StopListening(Keys.Down);
            InputManager.StopListening(Keys.Enter);
        }
    }
}
