using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace SmallEngine.Graphics
{
    public class Brush : IDisposable
    {
        internal SolidColorBrush OutlineColorBrush { get; private set; }
        internal SolidColorBrush FillColorBrush { get; private set; }

        public Color FillColor
        {
            get { return FillColorBrush.Color; }
            set { FillColorBrush.Color = value; }
        }

        public Color Outlinecolor
        {
            get { return OutlineColorBrush.Color; }
            set { OutlineColorBrush.Color = value; }
        }

        public float OutlineSize { get; set; }

        private Brush(Color? pFillColor, Color? pOutlineColor, float pOutlineSize, IGraphicsAdapter pTarget)
        {
            if(pTarget.Method == RenderMethods.DirectX)
            {
                var dx = (DirectXAdapter)pTarget;
                if(pFillColor.HasValue)
                {
                    var fc = pFillColor.Value;
                    FillColorBrush = new SolidColorBrush(dx.Context, new SharpDX.Color4(fc.R / 255f, fc.G / 255f, fc.B / 255f, fc.A / 255f));
                    FillColorBrush.Opacity = fc.A / 255f;
                }

                if(pOutlineColor.HasValue)
                {
                    var oc = pOutlineColor.Value;
                    OutlineColorBrush = new SolidColorBrush(dx.Context, new SharpDX.Color4(oc.R / 255f, oc.G / 255f, oc.B / 255f, oc.A / 255f));
                    OutlineColorBrush.Opacity = oc.A / 255f;
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            OutlineSize = pOutlineSize;
        }


        public static Brush CreateOutlineBrush(Color pColor, float pOutlineSize, IGraphicsAdapter pAdapter)
        {
            return new Brush(null, pColor, pOutlineSize, pAdapter);
        }

        public static Brush CreateFillBrush(Color pColor, IGraphicsAdapter pAdapter)
        {
            return new Brush(pColor, null, 0, pAdapter);
        }

        public static Brush Create(Color pFillColor, Color pOutlineColor, float pOutlineSize, IGraphicsAdapter pAdapter)
        {
            return new Brush(pFillColor, pOutlineColor, pOutlineSize, pAdapter);
        }

        public void Dispose()
        {
            FillColorBrush.Dispose();
            OutlineColorBrush.Dispose();
        }
    }
}
