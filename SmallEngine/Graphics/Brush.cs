using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SmallEngine.Graphics
{
    public class Brush : IDisposable
    {
        static Dictionary<Color, Brush> _cache = new Dictionary<Color, Brush>();

        public SharpDX.Direct2D1.SolidColorBrush ColorBrush { get; private set; }
        private Brush(Color pColor, SharpDX.Direct2D1.RenderTarget pTarget)
        {
            ColorBrush = new SharpDX.Direct2D1.SolidColorBrush(pTarget, new SharpDX.Mathematics.Interop.RawColor4(pColor.R, pColor.G, pColor.B, pColor.A));
            ColorBrush.Opacity = pColor.A / 255f;
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
