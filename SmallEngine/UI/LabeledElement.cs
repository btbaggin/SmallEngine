using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class LabeledElement : UIElement
    {
        public Font Font { get; set; }

        public UIElement Element { get; set; }

        public string Label { get; set; }

        public int LabelWidth { get; set; }

        public LabeledElement(string pLabel, UIElement pElement) : this(null, pLabel, pElement) { }

        public LabeledElement(string pName, string pLabel, UIElement pElement): base(pName)
        {
            Label = pLabel;
            LabelWidth = 100;
            Element = pElement;
            Font = Font.Create(UIManager.DefaultFontFamily, UIManager.DefaultFontSize, UIManager.DefaultFontColor, Game.Graphics);
        }

        public override Size MeasureOverride(Size pSize)
        {
            Element.Measure(pSize);
            return new Size(Element.DesiredSize.Width + LabelWidth, Element.DesiredSize.Height);
        }

        public override void ArrangeOverride(Rectangle pBounds)
        {
            Element.Arrange(new Rectangle(pBounds.X + LabelWidth, pBounds.Y, pBounds.Width - LabelWidth, pBounds.Height));
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawText(Label, new Rectangle(Bounds.X, Bounds.Y, LabelWidth, ActualHeight), Font);
            Element.Draw(pSystem);
        }

        public override void Update()
        {
            Element.Update();
        }
    }
}
