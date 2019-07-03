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
        public Brush Background { get; set; }

        public bool FillParent { get; set; }

        public PanelOrientation Orientation { get; set; }

        public Panel(PanelOrientation pOrientation) : this(null, pOrientation) { }

        public Panel(string pName, PanelOrientation pOrientation) : base(pName)
        {
            Orientation = pOrientation;
            FillParent = true;
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            if (Background != null) pSystem.DrawRect(Bounds, Background);
        }

        public override void Update() { }

        public void AddElement(UIElement pElement)
        {
            base.AddChild(pElement);
            InvalidateMeasure();
        }

        public void ClearElements()
        {
            //TODO doesn't work with named elements
            foreach(var c in Children)
            {
                c.Dispose();
            }
            Children.Clear();
        }

        public override Size MeasureOverride(Size pSize)
        {
            float width = 0;
            float height = 0;
            switch(Orientation)
            {
                case PanelOrientation.Vertical:
                    if (FillParent)
                    {
                        width = Width == 0 ? pSize.Width : Width;
                        height = Height == 0 ? pSize.Height : Height;
                        foreach (var c in Children) c.Measure(pSize);
                        return new Size(width, height);
                    }

                    foreach (var c in Children)
                    {
                        c.Measure(pSize);
                        height += c.DesiredSize.Height;
                        width = Math.Max(width, c.DesiredSize.Width);
                    }
                    break;

                case PanelOrientation.Horizontal:
                    if (FillParent)
                    {
                        width = Width == 0 ? pSize.Width : Width;
                        height = Height == 0 ? pSize.Height : Height;
                        foreach (var c in Children) c.Measure(pSize);
                        return new Size(width, height);
                    }

                    foreach (var c in Children)
                    {
                        c.Measure(pSize);
                        width += c.DesiredSize.Width;
                        height = Math.Max(height, c.DesiredSize.Height);
                    }
                    break;
            }

            return new Size(width, height);
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
                    p.X += c.ActualWidth;
                    width -= c.ActualWidth;
                }
                else if(Orientation == PanelOrientation.Vertical)
                {
                    p.Y += c.ActualHeight;
                    height -= c.ActualHeight;
                }
            }
        }
    }
}
