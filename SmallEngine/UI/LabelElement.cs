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
        public Font Font { get; set; }

        public string Text { get; set; }

        public LabelElement(string pText, string pFamily, float pSize, Color pColor)
        {
            Font = Game.Graphics.CreateFont(pFamily, pSize, pColor);
            Font.Alignment = Alignments.Center;
            Text = pText;
            SetLayout();
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            base.Draw(pSystem);
            pSystem.DrawText(Text, new Rectangle(Position, Width, Height), Font);
        }
    }
}
