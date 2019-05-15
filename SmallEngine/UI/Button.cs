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

        ButtonState _state;
        public Button(UIElement pContent) : this(null, pContent) { }

        public Button(string pName, UIElement pContent) : base(pName)
        {
            AddChild(pContent);

            Color = Color.Gray;
            DisabledColor = Color.GhostWhite;
            MouseOverColor = Color.LightGray;
            MouseDownColor = Color.DarkGray;
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            Brush brush;
            if (!Enabled) brush = pSystem.CreateBrush(DisabledColor);
            else
            {
                switch(_state)
                {
                    case ButtonState.Idle:
                        brush = pSystem.CreateBrush(Color);
                        break;

                    case ButtonState.MouseDown:
                        brush = pSystem.CreateBrush(MouseDownColor);
                        break;

                    case ButtonState.MouseOver:
                        brush = pSystem.CreateBrush(MouseOverColor);
                        break;

                    default:
                        throw new UnknownEnumException(typeof(ButtonState), _state);
                }
            }

            pSystem.DrawFillRect(Bounds, brush);
        }

        public override void Update(float pDeltaTime)
        {
            if(IsMouseOver())
            {
                if (Input.Mouse.KeyPressed(Input.MouseButtons.Left) && _state == ButtonState.MouseOver)
                {
                    _state = ButtonState.MouseDown;
                }
                else
                {
                    if (_state == ButtonState.MouseDown)
                    {
                        if(Input.Mouse.KeyUp(Input.MouseButtons.Left)) Clicked?.Invoke(this, new EventArgs());
                    }
                    else _state = ButtonState.MouseOver;
                }
            }
            else
            {
                _state = ButtonState.Idle;
            }
        }
    }
}
