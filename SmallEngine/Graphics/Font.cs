using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SmallEngine.Graphics
{
    public class Font : IDisposable
    {
        //TODO setters?
        public SharpDX.Direct2D1.SolidColorBrush Brush { get; private set; }
        public SharpDX.DirectWrite.TextFormat Format { get; private set; }
        public string Family { get { return Format.FontFamilyName; } }
        public float Size { get { return Format.FontSize; } }

        internal Font(SharpDX.DirectWrite.Factory pFactory, SharpDX.Direct2D1.RenderTarget pTarget, string pFamily, float pSize, Color pColor)
        {
            Format = new SharpDX.DirectWrite.TextFormat(pFactory, pFamily, pSize);
            Brush = new SharpDX.Direct2D1.SolidColorBrush(pTarget, new SharpDX.Mathematics.Interop.RawColor4(pColor.R, pColor.G, pColor.B, pColor.A));
        }

        public void Dispose()
        {
            Brush.Dispose();
            Format.Dispose();
        }
    }
}
