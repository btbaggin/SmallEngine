using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class Textblock : UIElement
    {
        private Font _font;
        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                InvalidateMeasure();
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                InvalidateMeasure();
            }
        }

        public Textblock(string pText) : this(null, pText) { }

        public Textblock(string pName, string pText) : base(pName)
        {
            Font = Font.Create(UIManager.DefaultFontFamily, UIManager.DefaultFontSize, UIManager.DefaultFontColor, Game.Graphics);
            Font.Alignment = Alignments.Center;
            Text = pText;
            Enabled = false;
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawText(Text, Bounds, Font);
        }

        public override void Update() { }

        public override Size MeasureOverride(Size pSize)
        {
            return Font.MeasureString(Text, pSize.Width);
        }

        public override void Dispose()
        {
            Font.Dispose();
            base.Dispose();
        }
    }
}
