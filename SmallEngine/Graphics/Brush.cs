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
        public SharpDX.Direct2D1.SolidColorBrush ColorBrush { get; private set; }
        internal Brush(Color pColor, SharpDX.Direct2D1.RenderTarget pTarget)
        {
            ColorBrush = new SharpDX.Direct2D1.SolidColorBrush(pTarget, new SharpDX.Mathematics.Interop.RawColor4(pColor.R, pColor.G, pColor.B, pColor.A));
        }

        public void Dispose()
        {
            ColorBrush.Dispose();
        }
    }
}
