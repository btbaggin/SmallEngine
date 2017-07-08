using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.Input;

namespace Evolusim.UI
{
    class ToggleButton
    {
        Brush _background;
        Brush _highlightBrush;
        Font _font;
        BitmapResource _image;
        string _text;
        float _imageSize;
        System.Drawing.RectangleF _rect;
        System.Drawing.RectangleF _textRect;

        bool _mouseOver;

        public bool IsSelected { get; private set; }

        public ToggleButton(BitmapResource pImage, string pText, float pX, float pY, float pWidth, float pHeight)
        {
            _image = pImage;
            _text = pText;

            _rect = new System.Drawing.RectangleF(pX, pY, pWidth, pHeight);
            _imageSize = pHeight - 8;

            _textRect = new System.Drawing.RectangleF(pX + pHeight, pY, pWidth - pHeight, pHeight);

            _highlightBrush = Game.Graphics.CreateBrush(System.Drawing.Color.Yellow);
            _background = Game.Graphics.CreateBrush(System.Drawing.Color.Green);
            _font = Game.Graphics.CreateFont("Arial", 18, System.Drawing.Color.White);
            _font.Alignment = Alignment.Center;

        }

        public void Draw(IGraphicsSystem pSystem)
        {
            pSystem.DrawFillRect(_rect, _background);

            pSystem.DrawBitmap(_image, 1, new Vector2(_rect.X + 4, _rect.Y + 4), new Vector2(_imageSize, _imageSize));
            pSystem.DrawText(_text, _textRect, _font);

            if(IsSelected)
            {
                pSystem.DrawRect(_rect, _highlightBrush, 5);
            }
        }

        public void Update(float pDeltaTime)
        {
            var p = InputManager.MousePosition;
            //TODO group so it doesnt unselect when clicking game board?
            _mouseOver = _rect.Contains(new System.Drawing.PointF(p.X, p.Y));
            if(InputManager.IsPressed(Mouse.Left))
            {
                IsSelected = _mouseOver;
            }
        }
    }
}
