using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.Input;
using SmallEngine.UI;

namespace Evolusim.UI
{
    class ToggleButton : UIElement
    {
        Brush _highlightBrush;
        Font _font;
        BitmapResource _image;
        string _text;
        float _imageSize;

        ToggleButtonGroup _group;

        public bool IsSelected { get; internal set; }

        public object Data { get; private set; }

        public ToggleButton(BitmapResource pImage, string pText, object pData) : this(pImage, pText, pData, null)
        {
        }

        public ToggleButton(BitmapResource pImage, string pText, object pData, ToggleButtonGroup pGroup)
        {
            WidthPercent = .8f;
            Height = 50;
            Margin = new Vector2(10, 0);
            _image = pImage;
            _text = pText;
            _imageSize = 32;
            Data = pData;

            _highlightBrush = Game.Graphics.CreateBrush(System.Drawing.Color.Yellow);
            _font = Game.Graphics.CreateFont("Arial", 18, System.Drawing.Color.White);
            _font.Alignment = Alignment.Center;

            if(pGroup != null)
            {
                _group = pGroup;
                _group.AddToGroup(this);
            }
            SetLayout();
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            var y = (Height - 32) / 2;
            var textRect = new System.Drawing.RectangleF(Position.X + _imageSize + y, Position.Y + y, Width - (_imageSize + y), 32);

            pSystem.DrawBitmap(_image, 1, new Vector2(Position.X + y, Position.Y + y), new Vector2(_imageSize, _imageSize));
            pSystem.DrawText(_text, textRect, _font);

            if(IsSelected)
            {
                pSystem.DrawRect(new System.Drawing.RectangleF(Position.X, Position.Y, Width, Height), _highlightBrush, 3);
            }
        }

        public override void Update(float pDeltaTime)
        {
            if(InputManager.KeyPressed(Mouse.Left) && IsMouseOver)
            {
                _group?.SetAllOff();
                IsSelected = IsMouseOver;
            }
        }
    }
}
