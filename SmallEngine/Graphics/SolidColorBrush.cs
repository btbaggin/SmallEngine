using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;

namespace SmallEngine.Graphics
{
    public class SolidColorBrush : Brush
    {
        readonly SharpDX.Direct2D1.SolidColorBrush _brush;
        internal override SharpDX.Direct2D1.Brush DirectXBrush => _brush;

        public Color Color
        {
            get { return _brush.Color; }
            set { _brush.Color = value; }
        }

        private SolidColorBrush(Color pColor, IGraphicsAdapter pTarget)
        {
            if (pTarget.Method == RenderMethods.DirectX)
            {
                var dx = (DirectXAdapter)pTarget;
                _brush = new SharpDX.Direct2D1.SolidColorBrush(dx.Context, new SharpDX.Color4(pColor.R / 255f, pColor.G / 255f, pColor.B / 255f, pColor.A / 255f));
                _brush.Opacity = pColor.A / 255f;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static SolidColorBrush Create(Color pColor)
        {
            return new SolidColorBrush(pColor, Game.Graphics);
        }
    }
}
