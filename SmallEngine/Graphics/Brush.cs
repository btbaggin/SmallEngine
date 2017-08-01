using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System.Drawing;

namespace SmallEngine.Graphics
{
    public class Brush : IDisposable
    {
        static Dictionary<Color, Brush> _cache = new Dictionary<Color, Brush>();

        public SolidColorBrush ColorBrush { get; private set; }
        public Color Color { get; private set; }
        private Brush(Color pColor, RenderTarget pTarget)
        {
            ColorBrush = new SolidColorBrush(pTarget, new SharpDX.Color4(pColor.R / 255f, pColor.G / 255f, pColor.B / 255f, pColor.A / 255f));
            ColorBrush.Opacity = pColor.A / 255f;
            Color = pColor;
        }

        internal static Brush Create(Color pColor, SharpDX.Direct2D1.RenderTarget pTarget)
        {
            if(!_cache.ContainsKey(pColor))
            {
                _cache.Add(pColor, new Brush(pColor, pTarget));
            }

            return _cache[pColor];
        }

        public void Dispose()
        {
            ColorBrush.Dispose();
        }
    }
}
