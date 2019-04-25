using System;
using System.Collections.Generic;
using SmallEngine;
using SmallEngine.Audio;
using SmallEngine.Graphics;
using SmallEngine.Input;

namespace Evolusim
{
    class MenuScene : Scene
    {
        readonly List<string> mItems;
        int mCurrentIndex;
        readonly bool _inGame;
        Font _titleFont;
        Font _font;
        Font _highlightFont;
        Brush _backgroundBrush;
        AudioResource _menu;

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

            _titleFont = Game.Graphics.CreateFont("Arial", 64, System.Drawing.Color.Blue);
            _font = Game.Graphics.CreateFont("Arial", 32, System.Drawing.Color.Black);
            _highlightFont = Game.Graphics.CreateFont("Arial", 40, System.Drawing.Color.White);
            _backgroundBrush = Game.Graphics.CreateBrush(System.Drawing.Color.FromArgb(128, 128, 128, 128));

            _menu = ResourceManager.Request<AudioResource>("menu");

            _titleFont.Alignment = Alignment.Center;
            _font.Alignment = Alignment.Center;
            _highlightFont.Alignment = Alignment.Center;
        }

        //TODO create menu rendercomponent
        //public override void Draw(IGraphicsAdapter pSystem)
        //{
        //    base.Draw(pSystem);

        //    pSystem.DrawFillRect(new Rectangle(0, 0, Game.Form.Width, Game.Form.Height), _backgroundBrush);

        //    pSystem.DrawText("Evolusim", new System.Drawing.RectangleF(0, 50, Game.Form.Width, 30), _titleFont);

        //    float y = 120;
        //    for (int i = 0; i < mItems.Count; i++)
        //    {
        //        System.Drawing.SizeF fontSize = _highlightFont.MeasureString(mItems[i], Game.Form.Width);
        //        var x = (Game.Form.Width / 2) - (fontSize.Width / 2);
        //        var currentY = y - (fontSize.Height / 2);

        //        if (mCurrentIndex == i)
        //        {
        //            pSystem.DrawText(mItems[i], new Rectangle(x, currentY, fontSize.Width, fontSize.Height), _highlightFont);
        //        }
        //        else
        //        {
        //            pSystem.DrawText(mItems[i], new Rectangle(x, currentY, fontSize.Width, fontSize.Height), _font);
        //        }
        //        y += fontSize.Height + 5;
        //    }
        //}

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime);

            if(InputManager.KeyPressed(Keys.Up))
            {
                if (mCurrentIndex == 0) mCurrentIndex = mItems.Count - 1;
                else mCurrentIndex--;
                AudioPlayer.Play(_menu);

            }
            else if(InputManager.KeyPressed(Keys.Down))
            {
                if (mCurrentIndex == mItems.Count - 1) mCurrentIndex = 0;
                else mCurrentIndex++;
                AudioPlayer.Play(_menu);
            }

            if(InputManager.KeyPressed(Keys.Enter))
            {
                EndScene();
            }
        }

        protected override void End()
        {
            base.End();

            if(!_inGame)
            {
                InputManager.StopListening(Keys.Up);
                InputManager.StopListening(Keys.Down);
            }
            InputManager.StopListening(Keys.Enter);

            _font.Dispose();
            _highlightFont.Dispose();

            switch (mCurrentIndex)
            {
                case 0:
                    if(!_inGame)
                        BeginScene(new GameScene());
                    break;
                case 1:
                    if (!_inGame)
                        BeginScene(new GameScene());
                    else
                        Game.Instance.IsPlaying = false;
                    break;

                case 2:
                    Game.Instance.IsPlaying = false;
                    break;

                default:
                    throw new ArgumentException("unknown index");
            }
        }
    }
}
