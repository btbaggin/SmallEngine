using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class LabelElement : UIElement
    {
        Font _font;
        string _text;
        public LabelElement(string pText, string pFamily, float pSize, System.Drawing.Color pColor)
        {
            _font = Game.Graphics.CreateFont(pFamily, pSize, pColor);
            _font.Alignment = Alignment.Center;
            _text = pText;
            SetLayout();
        }

        public override void Draw(IGraphicsSystem pSystem)
        {
            base.Draw(pSystem);
            pSystem.DrawText(_text, new System.Drawing.RectangleF(Position.X, Position.Y, Width, Height), _font);
        }
    }
}
