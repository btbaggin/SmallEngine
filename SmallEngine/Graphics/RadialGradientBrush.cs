using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;

namespace SmallEngine.Graphics
{
    public struct GradientColor
    {
        internal Color Color;
        internal float Position;

        public GradientColor(Color pColor, float pPosition)
        {
            Color = pColor;
            Position = pPosition;
        }
    }

    public sealed class RadialGradientBrush : Brush
    {
        readonly SharpDX.Direct2D1.RadialGradientBrush _brush;
        internal override SharpDX.Direct2D1.Brush DirectXBrush => _brush;

        private RadialGradientBrush(Vector2 pCenter, Vector2 pDirection, IGraphicsAdapter pAdapter, params GradientColor[] pColor)
        {
            if (pAdapter.Method == RenderMethods.DirectX)
            {
                var dx = (DirectXAdapter)pAdapter;
                var prop = new RadialGradientBrushProperties()
                {
                    Center = pCenter,
                    RadiusX = pDirection.X,
                    RadiusY = pDirection.Y
                };

                GradientStop[] stops = new GradientStop[pColor.Length];
                for(int i = 0; i < pColor.Length; i++)
                {
                    stops[i].Color = pColor[i].Color;
                    stops[i].Position = pColor[i].Position;
                }

                var grad = new GradientStopCollection(dx.Context, stops);
                _brush = new SharpDX.Direct2D1.RadialGradientBrush(dx.Context, ref prop, grad);
                _brush.Opacity = 1f;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static RadialGradientBrush Create(Vector2 pCenter, Vector2 pDirection, params GradientColor[] pColors)
        {
            return new RadialGradientBrush(pCenter, pDirection, Game.Graphics, pColors);
        }

        public override void Dispose()
        {
            DirectXBrush.Dispose();
        }
    }
}
