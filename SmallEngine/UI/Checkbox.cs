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
        public Color Background
        {
            get { return _backgroundBrush.FillColor; }
            set { _backgroundBrush.FillColor = value; }
        }

        public Color Foreground
        {
            get { return _foregroundBrush.FillColor; }
            set { _foregroundBrush.FillColor = value; }
        }

        public bool IsChecked { get; set; }

        readonly Brush _backgroundBrush, _foregroundBrush;

        public Checkbox() : this(null) { }

        public Checkbox(string pName) : base(pName)
        {
            _backgroundBrush = Brush.CreateFillBrush(Color.Gray, Game.Graphics);
            _foregroundBrush = Brush.CreateFillBrush(Color.Black, Game.Graphics);
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawRect(Bounds, _backgroundBrush);
            if(IsChecked)
            {
                pSystem.DrawLine(new Vector2(Bounds.Left, Bounds.Top), new Vector2(Bounds.Right, Bounds.Bottom), _foregroundBrush);
                pSystem.DrawLine(new Vector2(Bounds.Left, Bounds.Bottom), new Vector2(Bounds.Right, Bounds.Top), _foregroundBrush);
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
            _backgroundBrush.Dispose();
            _foregroundBrush.Dispose();
            base.Dispose();
        }
    }
}
