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

        public Checkbox() : this(null) { }

        public Checkbox(string pName) : base(pName)
        {
            Background = Color.Gray;
            Foreground = Color.Black;
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            pSystem.DrawFillRect(Bounds, pSystem.CreateBrush(Background));
            if(IsChecked)
            {
                var brush = pSystem.CreateBrush(Foreground);
                pSystem.DrawLine(new Vector2(Bounds.Left, Bounds.Top), new Vector2(Bounds.Right, Bounds.Bottom), brush);
                pSystem.DrawLine(new Vector2(Bounds.Left, Bounds.Bottom), new Vector2(Bounds.Right, Bounds.Top), brush);
            }
        }

        public override void Update(float pDeltaTime)
        {
            if (IsMouseOver() && Input.Mouse.KeyPressed(Input.MouseButtons.Left))
            {
                IsChecked = !IsChecked;
            }
        }
    }
}
