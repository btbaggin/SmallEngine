using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SharpDX.DirectWrite;

namespace SmallEngine.Graphics
{
    public enum Alignments
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

        public Color Color
        {
            get { return Brush.Color; }
            set { Brush.Color = value; }
        }

        public TextFormat Format { get; private set; }

        public string Family { get { return Format.FontFamilyName; } }

        public float Size { get { return Format.FontSize; } }

        public Alignments Alignment
        {
            get
            {
                switch (Format.TextAlignment)
                {
                    case TextAlignment.Justified:
                        return Alignments.Justified;

                    case TextAlignment.Leading:
                        return Alignments.Leading;

                    case TextAlignment.Trailing:
                        return Alignments.Trailing;

                    default:
                        return Alignments.Center;
                }
            }
            set
            {
                switch(value)
                {
                    case Alignments.Center:
                        Format.TextAlignment = TextAlignment.Center;
                        break;

                    case Alignments.Justified:
                        Format.TextAlignment = TextAlignment.Justified;
                        break;

                    case Alignments.Leading:
                        Format.TextAlignment = TextAlignment.Leading;
                        break;

                    case Alignments.Trailing:
                        Format.TextAlignment = TextAlignment.Trailing;
                        break;
                }
            }
        }
        #endregion

        readonly Factory _factory;

        #region Constructor
        private Font(string pFamily, float pSize, Color pColor, IGraphicsAdapter pAdapter)
        {
            if(pAdapter.Method == RenderMethods.DirectX)
            {
                var dx = (DirectXAdapter)pAdapter;
                _factory = dx.FactoryDWrite;
                Format = new TextFormat(dx.FactoryDWrite, pFamily, pSize);
                Brush = new SharpDX.Direct2D1.SolidColorBrush(dx.Context, pColor);
            }
            else
            {
                throw new NotImplementedException();
            }
            
        }

        public static Font Create(string pFamily, float pSize, Color pColor, IGraphicsAdapter pAdapter)
        {
            return new Font(pFamily, pSize, pColor, pAdapter);
        }
        #endregion  

        public Size MeasureString(string pText, float pWidth)
        {
            using (TextLayout l = new TextLayout(_factory, pText, Format, pWidth, Format.FontSize))
            {
                return new Size(l.Metrics.Width, l.Metrics.Height);
            }
        }

        public void Dispose()
        {
            Brush.Dispose();
            Format.Dispose();
        }
    }
}
