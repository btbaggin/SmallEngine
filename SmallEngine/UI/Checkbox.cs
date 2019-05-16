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
        enum ButtonState
        {
            Idle,
            MouseOver,
            MouseDown
        }

        public Color Background { get; set; }

        public Color Foreground { get; set; }

        public bool IsChecked { get; set; }

        readonly Brush _backgroundBrush, _foregroundBrush;

        public Checkbox() : this(null) { }

        public Checkbox(string pName) : base(pName)
        {
            Background = Color.Gray;
            Foreground = Color.Black;

            _backgroundBrush = Brush.CreateFillBrush(Background, Game.Graphics);
            _foregroundBrush = Brush.CreateFillBrush(Foreground, Game.Graphics);
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

        public override void Update(float pDeltaTime)
        {
            if (IsMouseOver() && Input.Mouse.ButtonPressed(Input.MouseButtons.Left))
            {
                IsChecked = !IsChecked;
            }
        }
    }
}
