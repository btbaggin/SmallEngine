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
        Brush _highlightBrush;
        Font _font;
        BitmapResource _image;
        string _text;
        float _imageSize;
        float _width, _height;

        ToggleButtonGroup _group;
        System.Drawing.RectangleF _rect;
        bool _mouseOver;

        public Vector2 Position { get; set; }

        public bool IsSelected { get; internal set; }

        public ToggleButton(BitmapResource pImage, string pText, float pWidth, float pHeight) : this(pImage, pText, pWidth, pHeight, null)
        {
        }

        public ToggleButton(BitmapResource pImage, string pText, float pWidth, float pHeight, ToggleButtonGroup pGroup)
        {
            _image = pImage;
            _text = pText;
            _width = pWidth;
            _height = pHeight;
            _imageSize = pHeight - 8;

            _highlightBrush = Game.Graphics.CreateBrush(System.Drawing.Color.Yellow);
            _font = Game.Graphics.CreateFont("Arial", 18, System.Drawing.Color.White);
            _font.Alignment = Alignment.Center;

            if(pGroup != null)
            {
                _group = pGroup;
                _group.AddToGroup(this);
            }
        }

        public void Draw(IGraphicsSystem pSystem)
        {
            _rect = new System.Drawing.RectangleF(Position.X, Position.Y, _width, _height);
            var textRect = new System.Drawing.RectangleF(Position.X + _height, Position.Y, _width - _height, _height);

            pSystem.DrawBitmap(_image, 1, new Vector2(_rect.X + 4, _rect.Y + 4), new Vector2(_imageSize, _imageSize));
            pSystem.DrawText(_text, textRect, _font);

            if(IsSelected)
            {
                pSystem.DrawRect(_rect, _highlightBrush, 3);
            }
        }

        public void Update(float pDeltaTime)
        {
            var p = InputManager.MousePosition;
            _mouseOver = _rect.Contains(new System.Drawing.PointF(p.X, p.Y));
            if(InputManager.IsPressed(Mouse.Left) && _mouseOver)
            {
                _group?.SetAllOff();
                IsSelected = _mouseOver;
            }
        }
    }
}
