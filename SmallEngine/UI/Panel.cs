using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public enum PanelOrientation
    {
        Horizontal,
        Vertical
    }

    public class Panel : UIElement
    {
        public PanelOrientation Orientation { get; set; }

        public Panel(PanelOrientation pOrientation) : this(null, pOrientation) { }

        public Panel(string pName, PanelOrientation pOrientation) : base(pName)
        {
            Orientation = pOrientation;
        }

        public override void Draw(IGraphicsAdapter pSystem) { }

        public override void Update(float pDeltaTime) { }

        public void AddElement(UIElement pElement)
        {
            base.AddChild(pElement);
        }

        public override Size MeasureOverride(Size pSize)
        {
            var desiredWidth = Orientation == PanelOrientation.Vertical ? pSize.Width : 0;
            var desiredHeight = Orientation == PanelOrientation.Horizontal ? pSize.Height : 0;
            foreach (var c in Children)
            {
                c.Measure(pSize);

                var s = c.DesiredSize;
                if(Orientation == PanelOrientation.Horizontal) desiredWidth += s.Width;
                else if (Orientation == PanelOrientation.Vertical) desiredHeight += s.Height;
            }

            return new Size(desiredWidth, desiredHeight);
        }

        public override void ArrangeOverride(Rectangle pBounds)
        {
            float width = 0;
            float height = 0;
            if(Orientation == PanelOrientation.Horizontal)
            {
                height = (int)pBounds.Height;
                width = (int)(DesiredSize.Width < pBounds.Width ? DesiredSize.Width : pBounds.Width);

            } else if (Orientation == PanelOrientation.Vertical)
            {
                width = (int)pBounds.Width;
                height = (int)(DesiredSize.Height < pBounds.Height ? DesiredSize.Height : pBounds.Height);
            }

            var p = Position;
            foreach(var c in Children)
            {
                c.Arrange(new Rectangle(p, width, height));
                if(Orientation == PanelOrientation.Horizontal)
                {
                    p.X += c.ActualSize.Width;
                    width -= c.ActualSize.Width;
                }
                else if(Orientation == PanelOrientation.Vertical)
                {
                    p.Y += c.ActualSize.Height;
                    height -= c.ActualSize.Height;
                }
            }
        }
    }
}
