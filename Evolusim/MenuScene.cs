using System;
using System.Collections.Generic;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.Input;

namespace Evolusim
{
    class MenuScene : Scene
    {
        private List<string> mItems;
        private int mCurrentIndex;
        private bool _inGame;
        private Font _titleFont;
        private Font _font;
        private Font _highlightFont;
        private Brush _backgroundBrush;

        public MenuScene(bool pInGame) : base()
        {
            if (!pInGame)
            {
                mItems = new List<string>(3)
                {
                    "New",
                    "Continue",
                    "Quit"
                };
            }
            else
            {
                mItems = new List<string>(2)
                {
                    "Resume",
                    "Quit"
                };
            }
            
            mCurrentIndex = 0;
            _inGame = pInGame;
        }

        protected override void Begin()
        {
            base.Begin();
            InputManager.Listen(Keys.Up);
            InputManager.Listen(Keys.Down);
            InputManager.Listen(Keys.Enter);

            _titleFont = Game.Graphics.CreateFont("Arial", 28, System.Drawing.Color.Blue);
            _font = Game.Graphics.CreateFont("Arial", 16, System.Drawing.Color.Black);
            _highlightFont = Game.Graphics.CreateFont("Arial", 18, System.Drawing.Color.White);
            _backgroundBrush = Game.Graphics.CreateBrush(System.Drawing.Color.Aqua);

            _titleFont.Alignment = Alignment.Center;
            _font.Alignment = Alignment.Center;
            _highlightFont.Alignment = Alignment.Center;
        }

        protected override void Draw(IGraphicsSystem pSystem)
        {
            base.Draw(pSystem);

            pSystem.DrawFillRect(new System.Drawing.RectangleF(0, 0, Game.Form.Width, Game.Form.Height), _backgroundBrush);

            pSystem.DrawText("Evolusim", new System.Drawing.RectangleF(0, 50, Game.Form.Width, 30), _titleFont);

            float y = 100;
            for(int i = 0; i < mItems.Count; i++)
            {
                if(mCurrentIndex == i)
                {
                    pSystem.DrawText(mItems[i], new System.Drawing.RectangleF(0, y, Game.Form.Width, 20), _highlightFont);
                }
                else
                {
                    pSystem.DrawText(mItems[i], new System.Drawing.RectangleF(0, y, Game.Form.Width, 20), _font);
                }
                y += 20;
            }
        }

        protected override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            if(InputManager.KeyPressed(Keys.Up))
            {
                if (mCurrentIndex == 0) mCurrentIndex = mItems.Count - 1;
                else mCurrentIndex--;
            }
            else if(InputManager.KeyPressed(Keys.Down))
            {
                if (mCurrentIndex == mItems.Count - 1) mCurrentIndex = 0;
                else mCurrentIndex++;
            }

            if(InputManager.KeyPressed(Keys.Enter))
            {
                SceneManager.EndScene();
            }
        }

        protected override void End()
        {
            base.End();

            InputManager.StopListening(Keys.Up);
            InputManager.StopListening(Keys.Down);
            InputManager.StopListening(Keys.Enter);

            _font.Dispose();
            _highlightFont.Dispose();

            switch (mCurrentIndex)
            {
                case 0:
                    if(!_inGame)
                    {
                        SceneManager.BeginScene(new GameScene());
                    }
                    break;
                case 1:
                    if(_inGame)
                    {
                        Game.IsPlaying = false;
                    }
                    else
                    {
                        SceneManager.BeginScene(new GameScene());
                    }
                    break;
                case 2:
                    Game.IsPlaying = false;
                    break;
                default:
                    throw new Exception("unknown index");
            }
        }
    }
}
