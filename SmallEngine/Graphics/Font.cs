using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SharpDX.DirectWrite;

namespace SmallEngine.Graphics
{
    public enum Alignment
    {
        Center,
        Justified,
        Leading,
        Trailing
    }

    public class Font : IDisposable
    {
        #region Properties
        internal SharpDX.Direct2D1.SolidColorBrush Brush { get; private set; }

        public TextFormat Format { get; private set; }

        public string Family { get { return Format.FontFamilyName; } }

        public float Size { get { return Format.FontSize; } }

        public Alignment Alignment
        {
            get
            {
                switch (Format.TextAlignment)
                {
                    case TextAlignment.Center:
                        return Alignment.Center;

                    case TextAlignment.Justified:
                        return Alignment.Justified;

                    case TextAlignment.Leading:
                        return Alignment.Leading;

                    case TextAlignment.Trailing:
                        return Alignment.Trailing;

                    default:
                        throw new Exception();
                }
            }
            set
            {
                switch(value)
                {
                    case Alignment.Center:
                        Format.TextAlignment = TextAlignment.Center;
                        break;

                    case Alignment.Justified:
                        Format.TextAlignment = TextAlignment.Justified;
                        break;

                    case Alignment.Leading:
                        Format.TextAlignment = TextAlignment.Leading;
                        break;

                    case Alignment.Trailing:
                        Format.TextAlignment = TextAlignment.Trailing;
                        break;
                }
            }
        }
        #endregion

        private Factory _factory;

        #region Constructor
        private Font(Factory pFactory, SharpDX.Direct2D1.SolidColorBrush pBrush, string pFamily, float pSize)
        {
            _factory = pFactory;
            Format = new TextFormat(pFactory, pFamily, pSize);
            Brush = pBrush;
        }

        internal static Font Create(Factory pFactory, SharpDX.Direct2D1.RenderTarget pTarget, string pFamily, float pSize, Color pColor)
        {
            return new Font(pFactory, new SharpDX.Direct2D1.SolidColorBrush(pTarget, new SharpDX.Mathematics.Interop.RawColor4(pColor.R, pColor.G, pColor.B, pColor.A)), pFamily, pSize);
        }
        #endregion  

        public SizeF MeasureString(string pText, float pWidth)
        {
            using (TextLayout l = new TextLayout(_factory, pText, Format, pWidth, Format.FontSize))
            {
                return new SizeF(l.Metrics.Width, l.Metrics.Height);
            }
        }

        public void Dispose()
        {
            Brush.Dispose();
            Format.Dispose();
        }
    }
}
