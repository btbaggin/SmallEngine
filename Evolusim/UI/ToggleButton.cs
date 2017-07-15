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
        System.Drawing.RectangleF _rect;

        public bool IsSelected { get; internal set; }

        public ToggleButton(BitmapResource pImage, string pText) : this(pImage, pText, null)
        {
        }

        public ToggleButton(BitmapResource pImage, string pText, ToggleButtonGroup pGroup)
        {
            WidthPercent = .8f;
            Height = 50;
            _image = pImage;
            _text = pText;
            _imageSize = 32;//pHeight - 8;

            _highlightBrush = Game.Graphics.CreateBrush(System.Drawing.Color.Yellow);
            _font = Game.Graphics.CreateFont("Arial", 18, System.Drawing.Color.White);
            _font.Alignment = Alignment.Center;

            if(pGroup != null)
            {
                _group = pGroup;
                _group.AddToGroup(this);
            }
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            _rect = new System.Drawing.RectangleF(Position.X, Position.Y, Width, Height);
            var textRect = new System.Drawing.RectangleF(Position.X + Height, Position.Y + Height / 2, Width - Height, Height / 2);

            pSystem.DrawBitmap(_image, 1, new Vector2(_rect.X + 4, _rect.Y + 4), new Vector2(_imageSize, _imageSize));
            pSystem.DrawText(_text, textRect, _font);

            if(IsSelected)
            {
                pSystem.DrawRect(_rect, _highlightBrush, 3);
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
