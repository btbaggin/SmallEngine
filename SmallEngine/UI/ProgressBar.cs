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
        public Brush Background { get; set; }

        public Brush Foreground { get; set; }

        public float MaxValue { get; set; }

        public float Value { get; set; }

        public bool Increment { get; set; }

        public Thickness BorderThickness { get; set; } = new Thickness(3);

        public float Progress
        {
            get { return Value / MaxValue; }
        }

        public ProgressBar(float pMax) : this(null, pMax) { }

        public ProgressBar(string pName, float pMax) : base(pName)
        {
            MaxValue = pMax;
            Background = SolidColorBrush.Create(Color.Gray);
            Foreground = SolidColorBrush.Create(Color.Red);
        }

        public override void Draw(IGraphicsAdapter pSystem)
        {
            var innerPos = new Vector2(Position.X + BorderThickness.Left, Position.Y + BorderThickness.Top);
            pSystem.DrawRect(new Rectangle(Position, ActualWidth, ActualHeight), Background);

            var scalar = Increment ? 1 - Progress : Progress;
            pSystem.DrawRect(new Rectangle(innerPos, (ActualWidth - BorderThickness.Width) * scalar, ActualHeight - BorderThickness.Height), Foreground);
        }

        public override void Update() { }

        public override Size MeasureOverride(Size pSize)
        {
            return pSize;
        }

        public override void Dispose()
        {
            base.Dispose();
            Background.Dispose();
            Foreground.Dispose();
        }
    }
}
