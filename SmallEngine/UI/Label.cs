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

        public Label(string pText) : this(null, pText) { }

        public Label(string pName, string pText) : base(pName)
        {
            Font = Font.Create(UIManager.DefaultFontFamily, UIManager.DefaultFontSize, UIManager.DefaultFontColor, Game.Graphics);
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
            InvalidateMeasure(); 
        }

        public override Size MeasureOverride(Size pSize)
        {
            var s = Font.MeasureString(Text, pSize.Width);
            return new Size(Math.Max(s.Width, pSize.Width), s.Height);
        }
    }
}
