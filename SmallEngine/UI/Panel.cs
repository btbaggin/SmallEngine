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

    public class Panel : ContainerElement
    {
        public bool FillParent { get; set; }

        public PanelOrientation Orientation { get; set; }

        public Panel(PanelOrientation pOrientation) : this(null, pOrientation) { }

        public Panel(string pName, PanelOrientation pOrientation) : base(pName)
        {
            Orientation = pOrientation;
            FillParent = true;
        }

        public override void Update() { }

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
            Vector2 p;
            switch(Orientation)
            {
                case PanelOrientation.Horizontal:
                    var childWidth = Children.Sum((e) => e.DesiredSize.Width + e.Margin.Width);
                    height = (int)pBounds.Height;
                    width = (int)(DesiredSize.Width < pBounds.Width ? DesiredSize.Width : pBounds.Width);

                    var x = Position.X;
                    switch (HorizontalContentAlignment)
                    {
                        case HorizontalAlignments.Center:
                            x += (pBounds.Width - childWidth) / 2;
                            break;

                        case HorizontalAlignments.Right:
                            x += (Bounds.Width - childWidth);
                            break;
                    }
                    p = new Vector2(x, Position.Y);
                    break;

                case PanelOrientation.Vertical:
                    var childHeight = Children.Sum((e) => e.DesiredSize.Height + e.Margin.Height);
                    width = (int)pBounds.Width;
                    height = (int)(DesiredSize.Height < pBounds.Height ? DesiredSize.Height : pBounds.Height);

                    var y = Position.Y;
                    switch(HorizontalContentAlignment)
                    {
                        case HorizontalAlignments.Center:
                            y += (pBounds.Height - childHeight) / 2;
                            break;

                        case HorizontalAlignments.Right:
                            y += (Bounds.Height - childHeight);
                            break;
                    }
                    p = new Vector2(Position.X, y);
                    break;

                default:
                    throw new UnknownEnumException(typeof(PanelOrientation), Orientation);
            }

            foreach(var c in Children)
            {
                c.Arrange(new Rectangle(p, width, height));
                if(Orientation == PanelOrientation.Horizontal)
                {
                    p.X += c.ActualWidth + c.Margin.Width;
                    width -= c.ActualWidth + c.Margin.Width;
                }
                else if(Orientation == PanelOrientation.Vertical)
                {
                    p.Y += c.ActualHeight + c.Margin.Height;
                    height -= c.ActualHeight + c.Margin.Height;
                }
            }
        }
    }
}
