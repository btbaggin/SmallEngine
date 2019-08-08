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

        public string Label { get; set; }

        public int LabelWidth { get; set; }

        public LabeledElement(string pLabel, UIElement pElement) : this(null, pLabel, pElement) { }

        public LabeledElement(string pName, string pLabel, UIElement pElement): base(pName)
        {
            AddChild(pElement);
            Label = pLabel;
            LabelWidth = 100;
            Font = Font.Create(UIManager.DefaultFontFamily, UIManager.DefaultFontSize, UIManager.DefaultFontColor);
        }

        public override Size MeasureOverride(Size pSize)
        {
            var content = Children[0];
            content.Measure(pSize);
            return new Size(content.DesiredSize.Width + LabelWidth, content.DesiredSize.Height);
        }

        public override void ArrangeOverride(Rectangle pBounds)
        {
            var content = Children[0];
            var rect = new Rectangle(pBounds.X + LabelWidth, pBounds.Y, pBounds.Width - LabelWidth, pBounds.Height);
            content.Arrange(rect);
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawText(Label, new Rectangle(Bounds.X, Bounds.Y, LabelWidth, ActualHeight), Font);
        }

        public override void Update() { }
    }
}
