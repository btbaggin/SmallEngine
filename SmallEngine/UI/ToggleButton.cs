using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class ToggleButton : UIElement
    {
        public EventHandler<EventArgs> Clicked { get; set; }

        bool _isToggled;
        public bool IsToggled
        {
            get { return _isToggled; }
            set
            {
                _isToggled = value;
                _background.Color = value ? ToggledColor : NonToggledColor;
            }
        }

        public Color ToggledColor { get; set; }

        public Color NonToggledColor { get; set; }

        public Pen Border { get; set; }

        Thickness _padding;
        public Thickness Padding
        {
            get { return _padding; }
            set
            {
                _padding = value;
                InvalidateMeasure();
            }
        }

        readonly SolidColorBrush _background;
        public ToggleButton(string pText) : this(null, new Label(pText) { HorizontalAlignment = HorizontalAlignments.Center }) { }

        public ToggleButton(UIElement pContent) : this(null, pContent) { }

        public ToggleButton(string pName, UIElement pContent) : base(pName)
        {
            AddChild(pContent);

            NonToggledColor = Color.Gray;
            ToggledColor = Color.DarkGray;

            _background = SolidColorBrush.Create(NonToggledColor);
            Border = Pen.Create(Color.Yellow, 2);
            _padding = new Thickness(3);
        }

        public override Size MeasureOverride(Size pSize)
        {
            var content = Children[0];
            content.Measure(pSize);
            return new Size(content.DesiredSize.Width + Padding.Width, content.DesiredSize.Height + Padding.Height);
        }

        public override void ArrangeOverride(Rectangle pBounds)
        {
            var content = Children[0];
            content.Arrange(new Rectangle(pBounds.X + Padding.Left, pBounds.Y + Padding.Top, pBounds.Width - Padding.Width, pBounds.Height - Padding.Height));
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            
            pSystem.DrawRect(Bounds, _background);
            if(IsToggled) pSystem.DrawRectOutline(Bounds, Border);
        }

        public override void Update()
        {
            if (Bounds.Contains(Input.Mouse.Position) && Input.Mouse.ButtonPressed(Input.MouseButtons.Left))
            {
                IsToggled = !IsToggled;
                Clicked?.Invoke(this, new EventArgs());
            }
        }

        public override void Dispose()
        {
            _background.Dispose();
            Border.Dispose();
            base.Dispose();
        }
    }
}
