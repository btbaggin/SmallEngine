using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class Label : UIElement
    {
        public Font Font { get; set; }

        public string Text { get; set; }

        public Label(string pText, string pFamily, float pSize, Color pColor) : base()
        {
            Font = Game.Graphics.CreateFont(pFamily, pSize, pColor);
            Font.Alignment = Alignments.Center;
            Text = pText;
            AllowFocus = false;
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawText(Text, Bounds, Font);
        }

        public override void Update(float pDeltaTime)
        {
            SetLayout(); 
        }

        public override System.Drawing.Size MeasureOverride(System.Drawing.Size pSize)
        {
            var s = Font.MeasureString(Text, pSize.Width);
            return new System.Drawing.Size((int)Math.Max(s.Width, pSize.Width), (int)s.Height);
        }
    }
}
