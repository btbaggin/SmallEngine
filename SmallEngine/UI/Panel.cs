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
            using (var tb = Debug.TimedBlock.Start("Measure_Panel"))
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
        }

        public override void ArrangeOverride(Rectangle pBounds)
        {
            float width = 0;
            float height = 0;
            var p = Position;
            switch(Orientation)
            {
                case PanelOrientation.Horizontal:
                    height = (int)pBounds.Height;
                    width = (int)(DesiredSize.Width < pBounds.Width ? DesiredSize.Width : pBounds.Width);
                    foreach (var c in Children)
                    {
                        c.Arrange(new Rectangle(p, width, height));
                        p.X += c.ActualWidth + c.Margin.Width;
                        width -= c.ActualWidth + c.Margin.Width;
                    }
                    break;

                case PanelOrientation.Vertical:
                    width = (int)pBounds.Width;
                    height = (int)(DesiredSize.Height < pBounds.Height ? DesiredSize.Height : pBounds.Height);
                    foreach (var c in Children)
                    {
                        c.Arrange(new Rectangle(p, width, height));
                        p.Y += c.ActualHeight + c.Margin.Height;
                        height -= c.ActualHeight + c.Margin.Height;
                    }
                    break;

                default:
                    throw new UnknownEnumException(typeof(PanelOrientation), Orientation);
            }

            float x = 0;
            float y = 0;
            switch(Orientation)
            {
                case PanelOrientation.Horizontal:
                    switch (HorizontalContentAlignment)
                    {
                        case HorizontalAlignments.Center:
                            x = width / 2;
                            break;

                        case HorizontalAlignments.Right:
                            x = width;
                            break;
                    }
                    break;

                case PanelOrientation.Vertical:
                    switch (HorizontalContentAlignment)
                    {
                        case HorizontalAlignments.Center:
                            y = height / 2;
                            break;

                        case HorizontalAlignments.Right:
                            y = height;
                            break;
                    }
                    break;
            }


            for (int i = 0; i < Children.Count; i++)
            {
                var c = Children[i];
                c.Position = new Vector2(c.Position.X + x, c.Position.Y + y);
            }
        }
    }
}
