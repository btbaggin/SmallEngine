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
        public EventHandler<EventArgs> ValueChanged { get; set; }

        public Brush Background { get; set; }

        public Pen Foreground { get; set; }

        bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if(value != _isChecked)
                {
                    _isChecked = value;
                    ValueChanged?.Invoke(this, new EventArgs());
                }
            }
        }

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
            return new Size(16);
        }

        public override void Dispose()
        {
            Background.Dispose();
            Foreground.Dispose();
            base.Dispose();
        }
    }
}
