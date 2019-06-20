using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Graphics;

namespace SmallEngine.UI
{
    public class ProgressBar : UIElement
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

        public float MaxValue { get; set; }
        public float Value { get; set; }

        public bool Increment { get; set; }

        public Thickness BorderThickness { get; set; } = new Thickness(3);

        public float Progress
        {
            get { return Value / MaxValue; }
        }

        readonly Brush _backgroundBrush;
        readonly Brush _foregroundBrush;
        public ProgressBar(float pMax) : this(null, pMax) { }

        public ProgressBar(string pName, float pMax) : base(pName)
        {
            MaxValue = pMax;
            _backgroundBrush = Brush.CreateFillBrush(Color.Gray, Game.Graphics);
            _foregroundBrush = Brush.CreateFillBrush(Color.Red, Game.Graphics);
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            var innerPos = new Vector2(Position.X + BorderThickness.Left, Position.Y + BorderThickness.Top);
            pSystem.DrawRect(new Rectangle(Position, ActualWidth, ActualHeight), _backgroundBrush);

            var scalar = Increment ? 1 - Progress : Progress;
            pSystem.DrawRect(new Rectangle(innerPos, (ActualWidth - BorderThickness.Width) * scalar, ActualHeight - BorderThickness.Height), _foregroundBrush);
        }

        public override void Update() { }

        public override Size MeasureOverride(Size pSize)
        {
            return new Size(pSize.Width, Height);
        }
    }
}
