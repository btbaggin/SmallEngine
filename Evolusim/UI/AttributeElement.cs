using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.UI;

namespace Evolusim.UI
{
    class AttributeElement : UIElement
    {
        Brush _background;
        Brush _foreground;
        float _value;
        float _min;
        float _max;
        public AttributeElement(string pAttribute, float pValue, float pMin, float pMax)
        {
            Orientation = ElementOrientation.Vertical;
            LabelElement le = new LabelElement(pAttribute, "Arial", 14, System.Drawing.Color.White) { WidthPercent = 1f, Height = 20 };
            AddChild(le, AnchorDirection.Top | AnchorDirection.Left, Vector2.Zero);
            WidthPercent = 1f;
            Height = 40;

            _value = pValue;
            _min = pMin;
            _max = pMax;
            _background = Game.Graphics.CreateBrush(System.Drawing.Color.Black);
            _foreground = Game.Graphics.CreateBrush(System.Drawing.Color.Gray);
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            base.Draw(pSystem);
            var w = Width * ((_value - _min) / (_max - _min));
            pSystem.DrawFillRect(new System.Drawing.RectangleF(Position.X, Position.Y + 20, Width, 10), _background);
            pSystem.DrawFillRect(new System.Drawing.RectangleF(Position.X + 1, Position.Y + 21, w, 8), _foreground);
        }
    }
}
