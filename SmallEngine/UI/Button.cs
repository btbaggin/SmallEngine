using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class Button : UIElement
    {
        enum ButtonState
        {
            Idle,
            MouseOver,
            MouseDown
        }

        public EventHandler<EventArgs> Clicked { get; set; }

        public Color Color { get; set; }

        public Color DisabledColor { get; set; }

        public Color MouseOverColor { get; set; }

        public Color MouseDownColor { get; set; }

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

        ButtonState _state;
        readonly SolidColorBrush _brush;
        public Button(string pText) : this(null, new Label(pText) { HorizontalAlignment = HorizontalAlignments.Center }) { }

        public Button(UIElement pContent) : this(null, pContent) { }

        public Button(string pName, UIElement pContent) : base(pName)
        {
            AddChild(pContent);

            Color = Color.Gray;
            DisabledColor = Color.GhostWhite;
            MouseOverColor = Color.LightGray;
            MouseDownColor = Color.DarkGray;
            _brush = SolidColorBrush.Create(Color);
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
            if (!Enabled) _brush.Color = DisabledColor;
            else
            {
                switch(_state)
                {
                    case ButtonState.Idle:
                        _brush.Color = Color;
                        break;

                    case ButtonState.MouseDown:
                        _brush.Color = MouseDownColor;
                        break;

                    case ButtonState.MouseOver:
                        _brush.Color = MouseOverColor;
                        break;

                    default:
                        throw new UnknownEnumException(typeof(ButtonState), _state);
                }
            }

            pSystem.DrawRect(Bounds, _brush);
        }

        public override void Update()
        {
            if(Bounds.Contains(Input.Mouse.Position))
            {
                if (Input.Mouse.ButtonPressed(Input.MouseButtons.Left) && _state == ButtonState.MouseOver)
                {
                    _state = ButtonState.MouseDown;
                }
                else
                {
                    if (_state == ButtonState.MouseDown)
                    {
                        if (Input.Mouse.ButtonUp(Input.MouseButtons.Left))
                        {
                            Clicked?.Invoke(this, new EventArgs());
                            _state = ButtonState.MouseOver;
                        }
                    }
                    else _state = ButtonState.MouseOver;
                }
            }
            else
            {
                _state = ButtonState.Idle;
            }
        }

        public override void Dispose()
        {
            _brush.Dispose();
            base.Dispose();
        }
    }
}
