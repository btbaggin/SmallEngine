using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class Panel : UIElement
    {
        public ElementOrientation Orientation { get; set; }

        public Panel(ElementOrientation pOrientation) : base()
        {
            Orientation = pOrientation;
        }

        public override void Draw(IGraphicsAdapter pSystem) { }

        public override void Update(float pDeltaTime) { }

        public override Size MeasureOverride(Size pSize)
        {
            int desiredWidth = Orientation == ElementOrientation.Vertical ? pSize.Width : 0;
            int desiredHeight = Orientation == ElementOrientation.Horizontal ? pSize.Height : 0;
            foreach (var c in Children)
            {
                c.Measure(pSize);

                var s = c.DesiredSize;
                if(Orientation == ElementOrientation.Horizontal) desiredWidth += s.Width;
                else if (Orientation == ElementOrientation.Vertical) desiredHeight += s.Height;
            }

            return new Size(desiredWidth, desiredHeight);
        }

        public override void ArrangeOverride(Rectangle pBounds)
        {
            int width = 0;
            int height = 0;
            if(Orientation == ElementOrientation.Horizontal)
            {
                height = (int)pBounds.Height;
                width = (int)(DesiredSize.Width < pBounds.Width ? DesiredSize.Width : pBounds.Width);

            } else if (Orientation == ElementOrientation.Vertical)
            {
                width = (int)pBounds.Width;
                height = (int)(DesiredSize.Height < pBounds.Height ? DesiredSize.Height : pBounds.Height);
            }

            var p = Position;
            foreach(var c in Children)
            {
                c.Arrange(new Rectangle(p, width, height));
                if(Orientation == ElementOrientation.Horizontal)
                {
                    p.X += c.ActualSize.Width;
                    width -= c.ActualSize.Width;
                }
                else if(Orientation == ElementOrientation.Vertical)
                {
                    p.Y += c.ActualSize.Height;
                    height -= c.ActualSize.Height;
                }
            }
        }
    }
}
