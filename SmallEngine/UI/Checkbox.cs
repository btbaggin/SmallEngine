using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class Checkbox : UIElement
    {
        public Brush Background { get; set; }

        public Pen Foreground { get; set; }

        public bool IsChecked { get; set; }

        public Checkbox() : this(null) { }

        public Checkbox(string pName) : base(pName)
        {
            Background = SolidColorBrush.Create(Color.Gray);
            Foreground = Pen.Create(Color.Black, 2);
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawRect(Bounds, Background);
            if(IsChecked)
            {
                pSystem.DrawLine(new Vector2(Bounds.Left, Bounds.Top), new Vector2(Bounds.Right, Bounds.Bottom), Foreground);
                pSystem.DrawLine(new Vector2(Bounds.Left, Bounds.Bottom), new Vector2(Bounds.Right, Bounds.Top), Foreground);
            }
        }

        public override void Update()
        {
            if (IsMouseOver() && Input.Mouse.ButtonPressed(Input.MouseButtons.Left))
            {
                IsChecked = !IsChecked;
            }
        }

        public override Size MeasureOverride(Size pSize)
        {
            var width = Math.Max(16, Width);
            var height = Math.Max(16, Height);
            return new Size(Math.Min(width, pSize.Width), Math.Min(height, pSize.Height));
        }

        public override void Dispose()
        {
            Background.Dispose();
            Foreground.Dispose();
            base.Dispose();
        }
    }
}
