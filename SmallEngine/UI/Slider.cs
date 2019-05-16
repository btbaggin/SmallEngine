using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;
using SmallEngine.Input;

namespace SmallEngine.UI
{
    public class Slider : UIElement
    {
        public int Min { get; set; }

        public int Max { get; set; }

        public int Value { get; set; }

        float _sliderRadius;
        int _sliderSize;
        public int SliderSize
        {
            get { return _sliderSize; }
            set
            {
                _sliderSize = value;
                _sliderRadius = value / 2f;
            }
        }

        public int BarHeight { get; set; }

        Vector2 _sliderPosition;
        bool _dragging;
        readonly Brush _barBrush, _sliderBrush;

        public Slider(int pMin, int pMax) : this(null, pMin, pMax) { }

        public Slider(string pName, int pMin, int pMax) : base(pName)
        {
            Min = pMin;
            Max = pMax;
            Value = Min;
            SliderSize = 20;
            BarHeight = 10;

            _barBrush = Brush.CreateFillBrush(Color.Gray, Game.Graphics);
            _sliderBrush = Brush.CreateFillBrush(Color.Red, Game.Graphics);
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            var y = Bounds.Top + ((Bounds.Height - BarHeight) / 2);
            pSystem.DrawRect(new Rectangle(Bounds.Left + _sliderRadius, y, Bounds.Width - SliderSize, BarHeight), _barBrush);

            pSystem.DrawElipse(_sliderPosition, _sliderRadius, _sliderBrush);
        }

        public override void Update(float pDeltaTime)
        {
            if(_sliderPosition.X == 0)
            {
                _sliderPosition = new Vector2(Position.X + _sliderRadius, Position.Y + Bounds.Height / 2);
            }

            var sliderBounds = new Rectangle(_sliderPosition.X, Position.Y, SliderSize, SliderSize);
            if (sliderBounds.Contains(Mouse.Position) && Mouse.ButtonDown(MouseButtons.Left)) _dragging = true;
            else if (Mouse.ButtonUp(MouseButtons.Left)) _dragging = false;

            if(_dragging)
            {
                var x = Mouse.Position.X - (SliderSize / 2);
                x = MathF.Clamp(x, Bounds.Left, Bounds.Right);

                _sliderPosition = new Vector2(x, Position.Y + Bounds.Height / 2);
                Value = (int)((x / (Position.X + Bounds.Width)) * (Max - Min));
            }
            System.Diagnostics.Debug.WriteLine(Value);
        }

        public override System.Drawing.Size MeasureOverride(System.Drawing.Size pSize)
        {
            return new System.Drawing.Size(pSize.Width, SliderSize);
        }
    }
}