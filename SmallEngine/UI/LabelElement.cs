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
        readonly Font _font;
        readonly string _text;
        public LabelElement(string pText, string pFamily, float pSize, Color pColor)
        {
            _font = Game.Graphics.CreateFont(pFamily, pSize, pColor);
            _font.Alignment = Alignment.Center;
            _text = pText;
            SetLayout();
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            base.Draw(pSystem);
            pSystem.DrawText(_text, new Rectangle(Position, Width, Height), _font);
        }
    }
}
