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
        #region Properties
        public int Min { get; set; }

        public int Max { get; set; }

        public int Value
        {
            get;
            set;
        }

        public float Percent
        {
            get { return (float)Value / (Max - Min); }
        }

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

        public Color BarColor
        {
            get { return _barBrush.FillColor; }
            set { _barBrush.FillColor = value; }
        }

        public Color SliderColor
        {
            get { return _sliderBrush.FillColor; }
            set { _sliderBrush.FillColor = value; }
        }

        public Font LabelFont { get; set; }
        #endregion

        Vector2 _sliderPosition;
        Rectangle _barBounds;
        float _labelWidth;
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

            LabelFont = Font.Create(UIManager.DefaultFontFamily, UIManager.DefaultFontSize, Color.White, Game.Graphics);
            LabelFont.Alignment = Alignments.Center;

            _barBrush = Brush.CreateFillBrush(Color.Gray, Game.Graphics);
            _sliderBrush = Brush.CreateFillBrush(Color.Red, Game.Graphics);
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            //Draw labels
            var y = Bounds.Top + ((ActualHeight - BarHeight) / 2);
            pSystem.DrawText(Min.ToString(), new Rectangle(Position.X, y, _labelWidth, ActualHeight), LabelFont);
            pSystem.DrawText(Max.ToString(), new Rectangle(_barBounds.Right, y, _labelWidth, ActualHeight), LabelFont);

            //Draw slider
            pSystem.DrawRect(_barBounds, _barBrush);
            pSystem.DrawElipse(_sliderPosition, _sliderRadius, _sliderBrush);

            //Show value if sliding
            if (_dragging)
            {
                pSystem.DrawText(Value.ToString(), new Rectangle(_sliderPosition, _labelWidth, ActualHeight), LabelFont);
            }
        }

        public override void Update()
        {
            //Calculate size for label
            _labelWidth = LabelFont.MeasureString(Min.ToString(), ActualWidth).Width;
            _labelWidth = Math.Max(LabelFont.MeasureString(Max.ToString(), ActualWidth).Width, _labelWidth);
            _labelWidth += _sliderRadius;

            //Calculate bounds for bar
            var x = Bounds.Left + _sliderRadius + _labelWidth;
            var y = Bounds.Top + ((ActualHeight - BarHeight) / 2);
            var width = ActualWidth - SliderSize - _labelWidth * 2;
            _barBounds = new Rectangle(x, y, width, BarHeight);

            //Initialize slider position to beginning if it isn't set
            if (_sliderPosition.X == 0)
            {
                var initialX = Percent * _barBounds.Width;
                _sliderPosition = new Vector2(Position.X + _sliderRadius + _labelWidth + initialX, Position.Y + ActualHeight / 2);
            }

            //Calculate bounds for slider
            var sliderBounds = new Rectangle(_sliderPosition.X, Position.Y, SliderSize, SliderSize);
            if (sliderBounds.Contains(Mouse.Position) && Mouse.ButtonDown(MouseButtons.Left)) _dragging = true;
            else if (Mouse.ButtonUp(MouseButtons.Left)) _dragging = false;

            //Move slider
            if(_dragging)
            {
                var sliderX = Mouse.Position.X - (SliderSize / 2);
                sliderX = MathF.Clamp(sliderX, _barBounds.Left, _barBounds.Right);

                _sliderPosition = new Vector2(sliderX, Position.Y + ActualHeight / 2);
                Value = (int)((sliderX - _barBounds.X) / _barBounds.Width * (Max - Min));
            }
        }

        public override Size MeasureOverride(Size pSize)
        {
            return new Size(pSize.Width, SliderSize);
        }

        public override void Dispose()
        {
            LabelFont.Dispose();
            _barBrush.Dispose();
            _sliderBrush.Dispose();
            base.Dispose();
        }
    }
}