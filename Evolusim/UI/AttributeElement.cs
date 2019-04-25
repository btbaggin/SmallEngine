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
        readonly Brush _background;
        readonly Brush _foreground;
        float _percent;

        public string Attribute { get; private set; }

        public AttributeElement(string pAttribute, float pPercent)
        {
            Attribute = pAttribute;
            Orientation = ElementOrientation.Vertical;
            LabelElement le = new LabelElement(Attribute, "Arial", 14, System.Drawing.Color.White) { WidthPercent = 1f, Height = 20 };
            AddChild(le, AnchorDirection.Top | AnchorDirection.Left, Vector2.Zero);
            WidthPercent = 1f;
            Height = 40;

            _percent = pPercent;
            _background = Game.Graphics.CreateBrush(System.Drawing.Color.Black);
            _foreground = Game.Graphics.CreateBrush(System.Drawing.Color.Gray);
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            base.Draw(pSystem);
            var w = Width * _percent;
            pSystem.DrawFillRect(new Rectangle(Position.X, Position.Y + 20, Width, 10), _background);
            pSystem.DrawFillRect(new Rectangle(Position.X + 1, Position.Y + 21, w, 8), _foreground);
        }

        public void UpdateValue(float pPercent)
        {
            _percent = pPercent;
        }
    }
}
